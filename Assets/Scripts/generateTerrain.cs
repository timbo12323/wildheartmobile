using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateTerrain : MonoBehaviour
{
    public int width = 256;  // Width of the terrain
    public int height = 256;  // Height of the terrain
    public float scale = 20f;  // Scale of the terrain
    public float mountainRadius = 100f;  // Radius of the mountain
    public float riverDepth = -10f;  // Depth of the river
    public float lakeDepth = -5f;  // Depth of the lake
    public int seed = 0;  // Seed for random number generation

    public Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        GenerateTerrain(terrain);
    }

    void GenerateTerrain(Terrain terrain)
    {
        // Set random seed
        Random.InitState(seed);

        // Generate heightmap
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                // Generate mountain
                float distanceFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(width / 2, height / 2));
                float mountainHeight = Mathf.Clamp01((mountainRadius - distanceFromCenter) / mountainRadius);
                heights[x, y] = mountainHeight;

                // Generate rivers
                float riverHeight = Mathf.PerlinNoise(xCoord, yCoord);
                heights[x, y] -= riverHeight * riverDepth;

                // Generate lakes
                float lakeHeight = Mathf.PerlinNoise(xCoord * 0.5f, yCoord * 0.5f);
                heights[x, y] -= lakeHeight * lakeDepth;
            }
        }

        // Apply the generated heightmap to the terrain
        terrain.terrainData.SetHeights(0, 0, heights);
    }
}