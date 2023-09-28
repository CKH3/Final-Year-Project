using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LODInfo{
    public int lod;
    public float visibleDistanceThreshold;
}

public class LODMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    System.Action updateCallback;

    public LODMesh(int lod, System.Action updateCallback){
        this.lod = lod;
        this.updateCallback = updateCallback;
    }

    void OnMeshDataReceived(MeshData meshData){
        this.mesh = meshData.CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public void RequestMeshData(MapData mapData){
        hasRequestedMesh = true;
        EndlessTerrain.mapGenerator.RequestMeshData(mapData,lod,OnMeshDataReceived);
    }
}
