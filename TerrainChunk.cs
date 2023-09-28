using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    GameObject meshObject;
    Vector2 position; //coordinate in world space
    Bounds bounds;

    MapData mapData;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;

    bool mapDataReceived;
    int previousLODIndex = -1;

    public TerrainChunk(Vector2 coordinate, int size, LODInfo[] detailLevels,Transform parent, Material material){
        this.detailLevels = detailLevels;
       
        position = coordinate * size;
        bounds = new Bounds(position,Vector2.one*size);
        Vector3 positionV3 = new Vector3(position.x,0,position.y);

        meshObject = new GameObject(String.Format("Terrain Chunk [{0},{1}]",coordinate.x,coordinate.y));
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = positionV3*EndlessTerrain.scale;
        meshObject.transform.SetParent(parent);
        meshObject.transform.localScale = Vector3.one * EndlessTerrain.scale;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for(int i = 0; i < detailLevels.Length; i++){
            lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
        }

        EndlessTerrain.mapGenerator.RequestMapData(position, OnMapDataReceived);
    }

    void OnMapDataReceived(MapData mapData){
        this.mapData = mapData;
        mapDataReceived = true;

        Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap,MapGenerator.mapChunkSize,MapGenerator.mapChunkSize);
        meshRenderer.material.mainTexture = texture;
        UpdateTerrainChunk();
    }

    public void UpdateTerrainChunk(){
        if(mapDataReceived){
            Vector2 viewerPosition = EndlessTerrain.viewerPosition;
            float viewerDistanceFromNeearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNeearestEdge <= EndlessTerrain.maxViewDist;
            if(visible){
                int lodIndex = 0;
                for(int i = 0; i < detailLevels.Length-1; i++){
                    if(viewerDistanceFromNeearestEdge > detailLevels[i].visibleDistanceThreshold){
                        lodIndex = i+1;
                    }else{
                        break;
                    }
                }

                if(lodIndex != previousLODIndex){
                    Debug.Log("Hello");
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if(lodMesh.hasMesh){
                        meshFilter.mesh = lodMesh.mesh;
                        previousLODIndex = lodIndex;
                        meshCollider.sharedMesh = lodMesh.mesh;
                        Debug.Log("Has Mesh!");
                    }else if(!lodMesh.hasRequestedMesh){
                        lodMesh.RequestMeshData(mapData);
                        Debug.Log("Mesh Requested!");
                    }
                }
                EndlessTerrain.terrainChunksVisibleLastUpdate.Add(this);
            }
            SetVisible(visible);
        }
    }

    public void SetVisible(bool visible){
        meshObject.SetActive(visible);
    }
}
