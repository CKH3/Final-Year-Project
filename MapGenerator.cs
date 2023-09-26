using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TerrainType{
    public string name;
    public float height;
    public Color color;
}

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{
        NoiseMap,
        ColorMap,
        MeshMap
    }

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public bool autoUpdate;
    public TerrainType[] regions;

    public void GenerateMap(){
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, seed, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth*mapHeight];
        for(int y = 0; y<mapHeight; y++){
            for(int x = 0; x<mapWidth; x++){
                float currentHeight = noiseMap [x,y];
                for(int i = 0; i < regions.Length; i++){
                    if(currentHeight <= regions[i].height){
                        colorMap[x + y * mapWidth] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay mapDisplay = GetComponent<MapDisplay>();
        
        switch(drawMode){
            case DrawMode.NoiseMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                break;
            case DrawMode.ColorMap:
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap,mapWidth,mapHeight));
                break;
            case DrawMode.MeshMap:
                mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMap(noiseMap,meshHeightMultiplier),TextureGenerator.TextureFromColorMap(colorMap,mapWidth,mapHeight));
                break;
            default:
                break;
        }
    }

    void OnValidate() {
        if(mapWidth<1){
            mapWidth = 1;
        }
        if(mapHeight<1){
            mapHeight = 1;
        }
        if(lacunarity < 1){
            lacunarity = 1;
        }
        if(octaves < 0){
            octaves = 0;
        }
    }
}
