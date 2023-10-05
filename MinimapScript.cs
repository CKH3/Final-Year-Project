using UnityEngine;

public class MinimapScript : MonoBehaviour {
    public Transform followPlayer;

    void Update(){
        transform.position = followPlayer.position + Vector3.up * EndlessTerrain.scale;
    }
}