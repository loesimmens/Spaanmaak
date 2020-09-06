using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapAGBuilder : MonoBehaviour
{
    public MapData mapData;
    public Tilemap tilemap;

    public char[,] map; //'g' is grass, 'h' is house, 'l' is ladder, 'r' is road, 's' is sand, 't' is tree, 'w' is water

    private int z;
    public int mapWidth;
    public int mapHeight;
    

    public Tile grassTile;
    public Tile houseTile;
    public Tile ladderTile;
    public Tile roadTile;
    public Tile sandTile;
    public Tile treeTile;
    public Tile waterTile;

    private Vector3Int ladderPos;

    // Start is called before the first frame update
    void Start()
    {
        z = 0;
        map = new char[mapWidth, mapHeight];

        //fill map with o's meaning empty tiles
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                int rand = UnityEngine.Random.Range(1, 100);

                if (rand <= 50)
                    map[i, j] = 's';
                else if (rand <= 90)
                    map[i, j] = 'g';
                else
                    map[i, j] = 'w';
                //PaintTile(i, j);
            }
        }

        int randX = UnityEngine.Random.Range(0, mapWidth);
        int randY = UnityEngine.Random.Range(0, mapHeight);

        while (map[randX, randY] != 's')
        {
            if (randX == mapWidth - 1)
            {
                randX = 0;
                if (randY == mapHeight - 1)
                    randY = 0;
                else
                    randY++;
            }
            else
                randX++;
        }

        map[randX, randY] = 'l';
        //PaintTile(randX, randY);

        Ladder ladder = new Ladder();
        ladder.UpPos = new Vector3Int(randX, randY, z);

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                mapData.Map[i,j] = map[i,j];
            }
        }
        mapData.Lddr = ladder;
    }

    private void PaintTile(int i, int j)
    {
        switch (map[i, j])
        {
            case 'g': tilemap.SetTile(new Vector3Int(i, j, z), grassTile); break;
            case 'h': tilemap.SetTile(new Vector3Int(i, j, z), houseTile); break;
            case 'l': tilemap.SetTile(new Vector3Int(i, j, z), ladderTile); break;
            case 'r': tilemap.SetTile(new Vector3Int(i, j, z), roadTile); break;
            case 's': tilemap.SetTile(new Vector3Int(i, j, z), sandTile); break;
            case 't': tilemap.SetTile(new Vector3Int(i, j, z), treeTile); break;
            case 'w': tilemap.SetTile(new Vector3Int(i, j, z), waterTile); break;
            default: Debug.Log(new Vector3Int(i, j, z) + " cannot be painted an allowed type of tile"); break;
        }
    }
}
