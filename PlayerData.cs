using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData{
    readonly string[] attributeKeys = {"level","vitality","strength","intelligence","dexterity","endurance","luck"};

    int _honor;
    Dictionary<string,int> _playerAttributesData;

    public PlayerData(){
        _honor = 0;
        _playerAttributesData = new Dictionary<string, int>();
        foreach(string key in attributeKeys){
            _playerAttributesData[key] = 1;
        }
    }

    


}