using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

[System.Serializable]
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}

//Store the map data for a new chunk
public struct MapData{
    public readonly float[,] heghtMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap){
        this.heghtMap = heightMap;
        this.colorMap = colorMap;
    }
}

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{
        NoiseMap,
        ColorMap,
        MeshMap,
        FalloutMap
    }
    public DrawMode drawMode;

    public Noise.NormalizeMode normalizeMode;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int EditorlevelOfDetail;//Change the number of vertices in each row
    public float noiseScale;//Scale of the size on noise map

    public int octaves;//Increase the complexity
    [Range(0,1)]
    public float persistance;//Change the amplitude
    public float lacunarity;// Change the frequency

    public int seed;//Seed for random number
    public Vector2 offset;//the position in perlin map(255x255)

    public float meshHeightMultiplier;//Change the height for each vertice on the mesh
    public AnimationCurve meshHeightCurve;//Adjust the height of vertices

    public bool useFalloutMap;
    public bool autoUpdate;
    public TerrainType[] regions;//Set the color for a certain height range

    float[,] falloutMap;
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    void Awake() {
        falloutMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    //Call by the MapEditorGenerator Class for testing
    public void DrawMapInEditor(){
        MapData mapData = GenerateMapData(Vector2.zero);
        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        switch(drawMode){
            case DrawMode.NoiseMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heghtMap));
                break;
            case DrawMode.ColorMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
                break;
            case DrawMode.MeshMap:
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMap(mapData.heghtMap,meshHeightMultiplier,meshHeightCurve,EditorlevelOfDetail),TextureGenerator.TextureFromColorMap(mapData.colorMap,mapChunkSize,mapChunkSize));
                break;
            case DrawMode.FalloutMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(falloutMap));
                break;
            default:
                break;
        }
    }

    public void RequestMapData(Vector2 centre,Action<MapData> callback){
        ThreadStart threadStart = delegate{
            MapDataThread(centre, callback);
        };
        new Thread(threadStart).Start();
    }

    //use thread to create height map and store as data
    void MapDataThread(Vector2 centre, Action<MapData> callback){
        MapData mapData = GenerateMapData(centre);
        lock(mapDataThreadInfoQueue){
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    //Use thread to create mesh and store as data
    public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback){
        ThreadStart threadStart = delegate{
            MeshDataThread(mapData, lod, callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lod, Action<MeshData> callback){
        MeshData meshData = MeshGenerator.GenerateTerrainMap(mapData.heghtMap,meshHeightMultiplier,meshHeightCurve,lod);
        lock(meshDataThreadInfoQueue){
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback,meshData));
        }
    }

    //Call callback function if the thread have finished data
    void Update(){
        if(mapDataThreadInfoQueue.Count > 0){
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++){
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if(meshDataThreadInfoQueue.Count > 0){
            for(int i = 0; i < meshDataThreadInfoQueue.Count; i++){
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    /**
    * Using perlin noise map and other parameter to form a 2D array "heightMap"
    * Base on the Height create a color map for each pixel;
    **/
    MapData GenerateMapData(Vector2 centre){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, seed, octaves, persistance, lacunarity, centre + offset, normalizeMode);

        Color[] colorMap = new Color[mapChunkSize*mapChunkSize];
        for(int y = 0; y<mapChunkSize; y++){
            for(int x = 0; x<mapChunkSize; x++){
                noiseMap[x,y] -= useFalloutMap? falloutMap[x,y]:0;
                float currentHeight = noiseMap [x,y];

                colorMap[x + y * mapChunkSize] = regions[0].color;
                for(int i = 0; i < regions.Length; i++){
                    if(currentHeight >= regions[i].height){
                        colorMap[x + y * mapChunkSize] = regions[i].color;
                    }else{
                        break;
                    }
                }
            }
        }
        
        return new MapData(noiseMap,colorMap);
    }

    //Prevent error input in Editor
    void OnValidate() {
        if(lacunarity < 1){
            lacunarity = 1;
        }
        if(octaves < 0){
            octaves = 0;
        }
        falloutMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    //Thread data storage structrue
    struct MapThreadInfo<T>{
        public readonly Action<T> callback;//Function that get back the output
        public readonly T parameter;//Output after thread finish

        public MapThreadInfo(Action<T> callback, T parameter){
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
