using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaterGenerator: MonoBehaviour{
    
    public float waterShowingDistance;
    [Range(1,240)]
    public float density;
    public Material waterMaterial;

    Dictionary<Vector2,GameObject> waterCoordinateDict = new Dictionary<Vector2, GameObject>();
    List<GameObject> lastFrameVisibleWater = new List<GameObject>();

    public void CreateWaterMap(Vector2 viewerPosition){
        //Calculate the viewer position in chunk coordinate
        float chunkSize = MapGenerator.mapChunkSize - 1;
        float scale = chunkSize*EndlessTerrain.scale/density;
        int viewerCoordinateX = Mathf.RoundToInt(viewerPosition.x / scale);
        int viewerCoordinateY = Mathf.RoundToInt(viewerPosition.y / scale);

        Vector2Int viewerCoordinate = new Vector2Int(viewerCoordinateX,viewerCoordinateY);
        // Debug.Log("Scale: " + scale);
        // Debug.Log("Player Coordinate in Water Map" + viewerCoordinate);
        int waterVisibleInViewDist = Mathf.RoundToInt(waterShowingDistance/scale*10);
        
        for(int i = 0; i < lastFrameVisibleWater.Count; i++){
            lastFrameVisibleWater[i].SetActive(false);
        }

        lastFrameVisibleWater.Clear();

        for(int y = -waterVisibleInViewDist; y <= waterVisibleInViewDist; y++){
            for(int x = -waterVisibleInViewDist; x <= waterVisibleInViewDist; x++){
                Vector2Int waterCoordinate = new Vector2Int(viewerCoordinateX+x,viewerCoordinateY+y);
                // Debug.Log(waterCoordinate);
                if(waterCoordinateDict.ContainsKey(waterCoordinate)){
                    waterCoordinateDict[waterCoordinate].SetActive(true);
                    lastFrameVisibleWater.Add(waterCoordinateDict[waterCoordinate]);
                }else{
                    waterCoordinateDict[waterCoordinate] = CreateWaterPlane(scale, waterCoordinate);
                }
            }
        }

    }

    public void ClearWaterMap(){
        while(transform.childCount > 0){
            foreach(Transform child in transform){
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public GameObject CreateWaterPlane(float scale,Vector2Int waterCoodinate){
        GameObject waterMap = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterMap.name = "Water Plane["+waterCoodinate.ToString()+"]";
        waterMap.transform.localScale = new Vector3(scale/10,1,scale/10);
        waterMap.transform.position = new Vector3(waterCoodinate.x+0.1f,0,waterCoodinate.y+0.1f)*scale;
        waterMap.GetComponent<MeshRenderer>().material = waterMaterial;
        waterMap.transform.SetParent(transform);
        waterMap.SetActive(false);
        return waterMap;
    }
}