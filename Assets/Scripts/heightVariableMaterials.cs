using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TerrainMaterial
{
    public Texture2D texture;
    public float minHeight;
    public float maxHeight;
}
public class heightVariableMaterials : MonoBehaviour
{
    public Terrain terrain;
    public List<TerrainMaterial> terrainMaterials;
    public bool bakeSplatmap = true;
    public string splatmapSavePath = "Assets/Splatmap.png";

    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        ApplyTerrainMaterials();
        if (bakeSplatmap)
            BakeSplatmap();
    }

    void ApplyTerrainMaterials()
    {
        TerrainData terrainData = terrain.terrainData;
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;

        List<TerrainLayer> terrainLayers = new List<TerrainLayer>();

        foreach (TerrainMaterial terrainMaterial in terrainMaterials)
        {
            TerrainLayer terrainLayer = new TerrainLayer();
            terrainLayer.diffuseTexture = terrainMaterial.texture;
            terrainLayer.tileSize = new Vector2(10f, 10f); // Adjust the tile size as needed

            terrainLayers.Add(terrainLayer);
        }

        terrainData.terrainLayers = terrainLayers.ToArray();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float normalizedHeight = heights[x, y];
                Vector3 terrainPos = new Vector3((float)x / (width - 1), 0, (float)y / (height - 1));

                int layerIndex = 0;
                foreach (TerrainMaterial terrainMaterial in terrainMaterials)
                {
                    if (normalizedHeight >= terrainMaterial.minHeight && normalizedHeight <= terrainMaterial.maxHeight)
                    {
                        float[,,] splatmapData = new float[1, 1, terrainData.terrainLayers.Length];
                        splatmapData[0, 0, layerIndex] = 1f;
                        terrainData.SetAlphamaps(x, y, splatmapData);

                        break;
                    }

                    layerIndex++;
                }
            }
        }
    }

    void BakeSplatmap()
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.alphamapWidth;
        int height = terrainData.alphamapHeight;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, width, height);

        Texture2D splatmapTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] splatmapColors = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float[] splatWeights = new float[terrainData.terrainLayers.Length];
                for (int layerIndex = 0; layerIndex < terrainData.terrainLayers.Length; layerIndex++)
                {
                    splatWeights[layerIndex] = splatmapData[x, y, layerIndex];
                }

                Color pixelColor = ConvertSplatWeightsToColor(splatWeights);
                splatmapColors[y * width + x] = pixelColor;
            }
        }

        splatmapTexture.SetPixels(splatmapColors);
        splatmapTexture.Apply();

        byte[] splatmapBytes = splatmapTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(splatmapSavePath, splatmapBytes);
        Debug.Log("Splatmap baked and saved at: " + splatmapSavePath);
    }


    Color ConvertSplatWeightsToColor(float[] splatWeights)
    {
        Color pixelColor = new Color();
        for (int i = 0; i < splatWeights.Length; i++)
        {
            pixelColor[i] = splatWeights[i];
        }
        return pixelColor;
    }
}