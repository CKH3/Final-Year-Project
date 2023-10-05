using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData{
    readonly string[] attributeKeys = {"level","honor","vitality","strength","intelligence","dexterity","endurance","luck"};

    Dictionary<string,int> _playerAttributesData;

    public PlayerData(){
        _playerAttributesData = new Dictionary<string, int>();
        foreach(string key in attributeKeys){
            _playerAttributesData[key] = 1;
        }
    }

    public void UpdatePlayerData(string key, int value, bool isReplace = false) {
        if(_playerAttributesData.ContainsKey(key)){
            int originalValue = _playerAttributesData[key];
            _playerAttributesData[key] = isReplace? value:originalValue+value;
        }
    }

    public int GetAttribute(string key){
        return _playerAttributesData[key];
    }

    public Dictionary<string,int> GetAttributes(){
        return _playerAttributesData;
    }


}