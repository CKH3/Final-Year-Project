using UnityEngine;


public class TreeGenerator: MonoBehaviour
{
    public const float coverage = 20;

    [SerializeField]
    GameObject[] treePrefabs;
    public LayerMask layerMask;
    [Range(0,10)]
    public float rndOffsetRange;

    public static bool[,] GenerateTreeMap(float[,] heightMap, int size, float scale)
    {
        bool[,] treeMap = new bool[size, size];
        float seed = Random.Range(0f, 100f);


        //Dont know why y need inverse
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xCoord = (float)x / size * scale + seed;
                float yCoord = (float)y / size * scale + seed;

                float noise = Mathf.PerlinNoise(xCoord, yCoord);

                // If the noise is greater than 0.5, place a tree
                treeMap[x, size-1-y] = noise > (1 - coverage/100) && heightMap[x,y] > 0.4f && heightMap[x,y] <= 0.6f;
            }
        }
        return treeMap;
    }

    public void CreateTrees(Transform parentChunk, bool[,] treeMap, Vector2 chunkPosition, float[,] heightMap){
            float scale = EndlessTerrain.scale;
            AnimationCurve meshHeightCurve = EndlessTerrain.mapGenerator.meshHeightCurve;
            float heightMultiplier = EndlessTerrain.mapGenerator.meshHeightMultiplier;
            // Debug.Log(chunkPosition);
            for(int y = 0; y < treeMap.GetLength(1); y++){
                for(int x = 0; x < treeMap.GetLength(0); x++){
                        //[Need Fixed] Dont know why the matrix need to be rotate
                        if(treeMap[x,y]){
                            GameObject rndTreePrefab = treePrefabs[Random.Range(0,treePrefabs.Length)];

                            Vector3 treePosition = new Vector3();
                            Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 180f), 0f);
                            float rndOffset = Random.Range(-rndOffsetRange,rndOffsetRange);
                            float treePositionX =(chunkPosition.x + x-treeMap.GetLength(0)/2)*scale;
                            float treePositionZ =(chunkPosition.y+ y-treeMap.GetLength(0)/2)*scale;
                            if (Physics.Raycast(new Vector3(treePositionX,100,treePositionZ), Vector3.down ,out RaycastHit hit, 200f, layerMask)) {
                                treePosition = new Vector3(treePositionX + rndOffset,hit.point.y,treePositionZ + rndOffset);
                            }
                            GameObject tree = Instantiate(rndTreePrefab,treePosition, randomRotation);
                            tree.transform.parent = parentChunk;
                            tree.transform.localScale *= scale/10;
                        }
                }
            }
    } 
}
