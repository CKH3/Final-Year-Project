using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
   public Renderer textureRender;
   public MeshFilter meshFilter;
   public MeshRenderer meshRenderer;
   public Transform meshTransform;

   public GameObject[] treePrefabs;
   public Transform trees;

     //Create Texture(Color)
     public void DrawTexture(Texture2D texture){
          textureRender.sharedMaterial.mainTexture = texture;
          textureRender.transform.localScale = new Vector3(texture.width,1,texture.height);
     }

     //Create Mesh(Shape)
     public void DrawMesh(MeshData meshData, Texture2D texture2D){
          meshFilter.sharedMesh = meshData.CreateMesh();
          meshRenderer.sharedMaterial.mainTexture = texture2D;
          if(TryGetComponent(out MapGenerator mapGenerator)){
               meshTransform.localScale = mapGenerator.useEndlessTerrainScale?Vector3.one*EndlessTerrain.scale:meshTransform.localScale;
          }
          // WaterGenerator.CreateWaterMap(Vector3.zero,waterMaterial);
     }

     public void DrawTree(bool[,] treeMap, float heightMultiplier, float[,] heightMap, AnimationCurve meshHeightCurve){
          float scale = EndlessTerrain.scale;
          for(int y = 0; y < treeMap.GetLength(1); y++){
               for(int x = 0; x < treeMap.GetLength(0); x++){
                    //[Need Fixed] Dont know why the matrix need to be rotate
                    if(treeMap[x,y]){
                         GameObject tree = Instantiate(treePrefabs[Random.Range(0,treePrefabs.Length)], new Vector3((x-treeMap.GetLength(0)/2)*scale,meshHeightCurve.Evaluate(heightMap[x,y])*heightMultiplier*10,(y-treeMap.GetLength(0)/2)*scale),Quaternion.identity);
                         tree.transform.SetParent(trees);
                    }
               }
          }
     }
}
