using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    //Generate a new mesh data
    public static MeshData GenerateTerrainMap(float[,] heightMap, float heightMultiplier, AnimationCurve _meshHeightCurve, int levelOfDetail){
        AnimationCurve meshHeightCurve = new AnimationCurve(_meshHeightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width-1)/-2f;
        float topLeftZ = (height-1)/2f;

        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail;
        int verticesPerLine = (width-1)/meshSimplificationIncrement + 1;
        MeshData meshData = new MeshData(verticesPerLine,verticesPerLine);
        int vertexIndex = 0;

        for(int y = 0; y < height; y += meshSimplificationIncrement){
            for(int x = 0; x < width; x += meshSimplificationIncrement){
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, meshHeightCurve.Evaluate(heightMap[x,y])*heightMultiplier, topLeftZ - y);                
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                if(x < width - 1 && y < height -1){
                    meshData.AddTriangle(vertexIndex, vertexIndex+verticesPerLine+1, vertexIndex+verticesPerLine);
                    meshData.AddTriangle(vertexIndex+verticesPerLine+1, vertexIndex, vertexIndex+1);
                }
                vertexIndex++;
            }
        }

        return meshData;
    }
}
