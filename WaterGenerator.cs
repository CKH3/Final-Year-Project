using UnityEngine;

public static class WaterGenerator{
    public static Material waterMaterial;

    public static void CreateWaterMap(Vector2 viewerPosition){
        //Calculate the viewer position in chunk coordinate
        float chunkSize = MapGenerator.mapChunkSize - 1;
        int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x/chunkSize);
        int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y/chunkSize);
    }
}