using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
* Use to Generate Endless Terrain base on the View Distance
* Each Terrain is 240*240 size
* viewer is the center of the showing Terrain
**/
public class EndlessTerrain : MonoBehaviour
{   
    public const float scale = 15;
    const float viewerMoveThresholdForChunkUpdate = 25f;

    public LODInfo[] detailLevels;
    public static float maxViewDist; // Adjust the number of showing terrain
    
    public Transform viewer;
    public Material mapMaterial;

    public static Vector2 viewerPosition;
    public static MapGenerator mapGenerator;
    public static WaterGenerator waterGenerator;
    public static TreeGenerator treeGenerator;

    Vector2 previousViewerPosition;
    int chunkSize; //240
    int chunkVisibleInViewDist; //number of chunk can see in one direction

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();// keep the reference for all generated terrain chunks
    public static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        waterGenerator = FindObjectOfType<WaterGenerator>();
        treeGenerator = FindObjectOfType<TreeGenerator>();
        maxViewDist = detailLevels[detailLevels.Length-1].visibleDistanceThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunkVisibleInViewDist = Mathf.RoundToInt(maxViewDist/chunkSize);
        viewerPosition = Vector3.zero;

        UpdateVisibleChunks();
    }

    // Update is called once per frame
    // Calling the UpdateVisibleChunks every frame
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z)/scale; 
        float movingDistanceOfViewer = Vector2.Distance(viewerPosition,previousViewerPosition);
        if(movingDistanceOfViewer > viewerMoveThresholdForChunkUpdate){
            UpdateVisibleChunks();
            previousViewerPosition = viewerPosition;
        }

        //Update Water Rendering
        waterGenerator.CreateWaterMap(viewerPosition*scale);
    }

    //Check whether the terrain chunks is visible or not
    void UpdateVisibleChunks(){
        //Disactive all the chunks visible in previous frame
        for(int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++){
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }

        //Calculate the viewer position in chunk coordinate
        int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
        int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y/chunkSize);

        Debug.Log(currentChunkCoordinateX+","+currentChunkCoordinateY);

        //Get all the chunk that are within the view distance and make them visable
        for(int yOffset = -chunkVisibleInViewDist; yOffset <= chunkVisibleInViewDist; yOffset++){
            for(int xOffset = -chunkVisibleInViewDist; xOffset <= chunkVisibleInViewDist; xOffset++){
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkCoordinateX + xOffset, currentChunkCoordinateY + yOffset);

                if(terrainChunkDictionary.ContainsKey (viewedChunkCoordinate)){
                    terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk();
                }else{
                    //If that chunk is not create, create a new chunk
                    terrainChunkDictionary.Add(viewedChunkCoordinate, new TerrainChunk(viewedChunkCoordinate, chunkSize, detailLevels, transform, mapMaterial));
                }
            }
        }

    }
}
