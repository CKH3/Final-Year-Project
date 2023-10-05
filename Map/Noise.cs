using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode{
        Local,
        Global
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode){
        float[,] noiseMap = new float[mapWidth,mapHeight];
        scale = (scale <= 0)? 0.0001f:scale;
        
        System.Random random = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for(int i = 0; i < octaves; i++){
            octaveOffsets[i] = new Vector2(random.Next(-100000, 100000) + offset.x, random.Next(-100000, 100000) - offset.y);
            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth/2f;
        float halfHeight = mapHeight/2f;        

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++){
                    float sampleX = (x-halfWidth + octaveOffsets[i].x)/scale*frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y)/scale*frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX,sampleY)*2-1;
                    noiseHeight += perlinValue*amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxLocalNoiseHeight){
                    maxLocalNoiseHeight = noiseHeight;
                }else if(noiseHeight < minLocalNoiseHeight){
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }

        for(int y = 0; y < mapHeight; y++){
            for(int x = 0; x < mapWidth; x++){
                if(normalizeMode == NormalizeMode.Local){
                    noiseMap[x,y] = Mathf.InverseLerp(minLocalNoiseHeight,maxLocalNoiseHeight,noiseMap[x,y]);
                } else {
                    float normalizeHeight = (noiseMap [x,y] + 1)/maxPossibleHeight;
                    noiseMap[x,y] = Mathf.Clamp(normalizeHeight,0,int.MaxValue);
                }
            }
        }
        return noiseMap;  
    }
}
