using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{

    public static class NoiseGenerator
    {

        public enum NormalizedModel { Local, Global }

        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizedModel normalizedMode)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            if (scale == 0)
            {
                scale = 0.0001f;
            }

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves]; // because now octaves represent the total of the Vector2, so It should be a passitive int or z
            float maxPossibleHeight = 0;
            float amplitude = 1; //up down value
            float frequency = 1; //
            for (int j = 0; j < octaves; ++j)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffsets[j] = new Vector2(offsetX, offsetY);
                maxPossibleHeight += amplitude;
                amplitude *= persistance;
            }

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {

                    amplitude = 1; //up down value
                    frequency = 1; //
                    float noiseHeight = 0;

                    // x - halfWidth and y - halfHeight means move the origin point to the center of the map when we scale the map 
                    // when we scale we move to the origiin to the center, then we zoom in or zoom out.
                    for (int i = 0; i < octaves; ++i)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency * 1.00003f; //just wanna make sure sampleX and sampleY not intger all the time.
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency * 1.00003f;
                        //the perlinValue is the greyscale values represent values from 0..1
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;  // now the value become [-1,1]
                                                                                          //Debug.Log(perlinValue);

                        //octaves 1, main outline. octaves 2,boulders. octaves 3 small rocks.
                        noiseHeight += perlinValue * amplitude; // here those three stack overleped.
                                                                // the first noiseHeight will be perlinValue, the second noisHeight will be (noiseHeight + noiseHeight * persistance)...(noiseHeight + noiseHeight * persistance^2);
                        amplitude *= persistance; //a1 =p^0; a2 = p^1; a3 = p^2; change the undown amplitude value of each stack
                        frequency *= lacunarity; //f1 = l^0; f2 = l^1; f3 = l^2; change the frequency for each stack.
                    }

                    // When an set numbers is not increase order, or decrease order.
                    // and we wanta get the min and max....we can use this mothed.
                    //   Max)------------------------->
                    //           noiseHeight
                    //<------------------------(Min
                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < mapHeight; ++y)
            {
                for (int x = 0; x < mapWidth; ++x)
                {
                    //if noiseMap[x,y] = minNoiseHeight, then return 0; if...... return 1;
                    //before the range of noiseMap[x,y] is [minNoiseHeight,MaxNoiseHeight].
                    //now we covert the range to [0,1] based on the origin noiseMap[x,y]
                    if (normalizedMode == NormalizedModel.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                    else
                    {
                        float normalizeHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizeHeight, 0, int.MaxValue);
                    }
                }
            }
            return noiseMap;
        }

    }

    public static class FalloffGenerator
    {

        public static float[,] GenerateFallofMap(int size)
        {
            float[,] map = new float[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //Range from -1 to 1. and detece which one is closer to the edge
                    float x = (i / (float)size) * 2 - 1;
                    float y = (j / (float)size) * 2 - 1;
                    float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                    map[i, j] = Evaluate(value);
                }
            }
            return map;
        }

        static float Evaluate(float value)
        {
            float a = 3;
            float b = 2.2f;

            //Google unitled graph to draw the curve
            return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
        }

        public static void ApplyFullOfMap(ref float[,] mapData, float[,] fullOfMap)
        {
            float size = mapData.GetLength(0);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    mapData[x, y] = Mathf.Clamp01(mapData[x, y] - fullOfMap[x, y]);
                }
            }
        }
    }

    public static class TerrainMapDataGenerator
    {
        public static TerrainMapData GenerateTerrainMapData(Vector2 center, int verticesSize, int seed, float noiseScale, int octaves,
                                                     float persistance, float lacunarity, Vector2 offset, NoiseGenerator.NormalizedModel model, TerrainType[] regions = null, float[,] fallOfMap = null, AnimationCurve curve = null)
        {
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(verticesSize, verticesSize, seed, noiseScale, octaves, persistance, lacunarity, center + offset, model);
            Color[] colourMap = new Color[verticesSize * verticesSize];

            for (int y = 0; y < verticesSize; y++)
            {
                for (int x = 0; x < verticesSize; x++)
                {
                    if (curve != null) { noiseMap[x, y] = curve.Evaluate(noiseMap[x, y]); }

                    if (fallOfMap != null)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - fallOfMap[x, y]);
                    }

                    if (regions == null) { continue; }

                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight >= regions[i].height)
                        {
                            colourMap[y * verticesSize + x] = regions[i].colour;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return new TerrainMapData(noiseMap, colourMap);
        }
    }

    public static class TextureGenerator
    {
        public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colourMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            Color[] colourData = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourData[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                }
            }
            return TextureFromColourMap(colourData, width, height);
        }
    }

    public static class TextureDataGenerator
    {
        public static float[,] GenerateHeightFromTexture(Texture2D tex2D,int verticesPerRow)
        {
            float[,] heights = new float[verticesPerRow, verticesPerRow];
            for (int z = 0; z < verticesPerRow; z++)
            {
                for (int x = 0; x < verticesPerRow; x++)
                {
                    float u = (float)x / verticesPerRow;
                    float v = (float)z / verticesPerRow;
                    heights[x, z] = tex2D.GetPixelBilinear(u, v).r;
                }
            }
            return heights;
        }

        public static float[,] GenerateHeightFromTexture(Texture2D tex2D, int verticesPerRow, float scale, Vector2 offset)
        {
            float[,] heights = new float[verticesPerRow, verticesPerRow];
            for (int z = 0; z < verticesPerRow; z++)
            {
                for (int x = 0; x < verticesPerRow; x++)
                {
                    
                    float u = (float)x / verticesPerRow;
                    float v = (float)z / verticesPerRow;

                    u += offset.x;
                    v += offset.y;
                    
                    heights[x, z] = tex2D.GetPixelBilinear(u, v).r;
                }
            }
            return heights;
        }
    }
        

    public static class TextureArrayGenerator
    {
        public static Texture2DArray GenerateTextureArray(Texture2D[] textures, int size, TextureFormat format)
        {
            Texture2DArray textureArray = new Texture2DArray(size, size, textures.Length, format, true);
            for (int i = 0; i < textures.Length; i++)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);
            }
            textureArray.Apply();
            return textureArray;
        }
    }

    public struct TerrainMapData
    {
        public readonly float[,] heightMap;
        public readonly Color[] colourMap;

        public TerrainMapData(float[,] _heightMap, Color[] _colourMap)
        {
            heightMap = _heightMap;
            colourMap = _colourMap;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color colour;
    }

}
