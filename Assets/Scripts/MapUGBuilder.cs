using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR.WSA.Input;

//Author: Loes Immens
//creates a dungeon with one ladder and multiple rooms connected by paths and doors
public class MapUGBuilder : MonoBehaviour
{
    public MapData mapData;
    public Tilemap tilemap;

    public char[,] map; //'o' is empty, 'f' is floor, 'w' is wall, 'd' is door, 'l' is ladder
    private char[,] pathMap;
    private char[,] roomOrPath;

    private int z;
    public int mapWidth;
    public int mapHeight;

    public int minRoomWidth;
    public int maxRoomWidth;
    public int minRoomHeight;
    public int maxRoomHeight;
    
    public Tile floorTile;
    public Tile wallTile;
    public Tile doorTile;
    public Tile ladderTile;
    public Tile caveTile;

    public int tries;
    private int randX;
    private int randY;
    private int randWidth;
    private int randHeight;
    private bool roomBuildMoved;

    public List<Room> rooms;
    private int nRooms;
    private bool startingRoom;
    private bool endingRoom;

    private Vector3Int ladderPos;

    // Start is called before the first frame update
    void Start()
    {
        z = 0;
        startingRoom = false;
        endingRoom = false;
        map = new char[mapWidth, mapHeight];
        pathMap = new char[mapWidth, mapHeight];
        roomOrPath = new char[mapWidth, mapHeight];
        roomBuildMoved = false;

        //fill map with o's (empty tiles)
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] = 'o';
                pathMap[i, j] = 'o';
                roomOrPath[i, j] = 'o';
            }
        }

        for (int k = 0; k < tries; k++)
        {
            randX = UnityEngine.Random.Range(0, mapWidth - 4);
            randY = UnityEngine.Random.Range(0, mapHeight - 4);
            randWidth = UnityEngine.Random.Range(minRoomWidth, maxRoomWidth);
            randHeight = UnityEngine.Random.Range(minRoomHeight, maxRoomHeight);
            roomBuildMoved = false;
            if (IfAbleBuildRoom(randWidth, randHeight, randX, randY))
            {
                Room r = ScriptableObject.CreateInstance("Room") as Room;
                r.X = randX;
                r.Y = randY;
                r.Width = randWidth;
                r.Height = randHeight;
                r.StartingRoom = false;
                r.EndingRoom = false;
                if (!endingRoom)
                {
                    r.EndingRoom = true;

                    if (!startingRoom)
                    {
                        r.StartingRoom = true;
                    }
                }

                if (nRooms > 0)
                    ConnectRoom(r, rooms[nRooms - 1]);
                else
                {
                    for (int i = r.X + 1; i < r.X + r.Width - 1; i++)
                        for (int j = r.Y + 1; j < r.Y + r.Height - 1; j++)
                        {
                            pathMap[i, j] = 'c';
                        }
                    r.Connected = true;
                }

                rooms.Add(r);
                nRooms++;
            }
            else
            {
                RemoveUnusedDoors(randWidth, randHeight, randX, randY);
            }
        }
        
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                if (map[i, j] == 'f' && pathMap[i,j] == 'n')
                    map[i, j] = 'w';
                if(!startingRoom && map[i,j] == 'f' && roomOrPath[i,j] == 'r')
                {
                    ladderPos = new Vector3Int(i + 1, j + 1, z);
                    map[i + 1, j + 1] = 'l';
                    map[ladderPos.x, ladderPos.y] = 'l';
                    startingRoom = true;
                    
                }
                if(map[i,j] == 'o')
                    map[i, j] = 'w';
            }
        }

        mapData.Map = new char[mapWidth,mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                mapData.Map[i, j] = map[i, j];
            }
        }

        mapData.TileMap = tilemap;
        mapData.Rooms = rooms;
        mapData.NRooms = nRooms;

        Ladder ladder = new Ladder
        {
            DownPos = new Vector3Int(ladderPos.x, ladderPos.y, z)
        };
        mapData.Lddr = ladder;

        for(int i = 0; i < mapWidth; i++)
            for(int j = 0; j < mapHeight; j++)
            {
                PaintTile(i, j);
            }

    }

    private void PaintTile(int i, int j)
    {
        switch (map[i, j])
        {
            case 'd': tilemap.SetTile(new Vector3Int(i, j, z), doorTile); break;
            case 'f': tilemap.SetTile(new Vector3Int(i, j, z), floorTile); break;
            case 'l': tilemap.SetTile(new Vector3Int(i, j, z), ladderTile); break;
            case 'w': tilemap.SetTile(new Vector3Int(i, j, z), wallTile); break;
            default: Debug.Log(new Vector3Int(i, j, z) + " can only be painted allowed types of tile"); break;
        }
    }

    private void RemoveUnusedDoors(int width, int height, int x, int y)
    {
        for(int i = x; i < x + width && i < mapWidth; i++)
            for(int j = y; j < y + height && j < mapHeight; j++)
            {
                if(map[i,j] == 'd') //if door tile
                {
                    if (!((map[i - 1, j] == 'w' && map[i + 1, j] == 'w')
                        || (map[i, j - 1] == 'w' && map[i, j + 1] == 'w'))) //without necessary wall
                        map[i, j] = 'f'; //turn door into floor again
                }
            }
    }

    private void ConnectRoom(Room fromRoom, Room toRoom)
    {
        bool connected = false;
        Vector3Int fromPos = PointInRoom(fromRoom);
        Vector3Int toPos = PointInRoom(toRoom);

        for(int i = fromRoom.X; i < fromRoom.X + fromRoom.Width; i++)
            for(int j = fromRoom.Y; j < fromRoom.Y + fromRoom.Height; j++)
            {
                if (pathMap[i, j] == 'c')
                {
                    connected = true;
                    Debug.Log("no path needed, room is already connected");
                    break;
                }
            }
        if (!connected) //if not connected yet, find path to connected area
        {
            if (!FindPath(fromPos, fromPos, toPos, 0))
            {
                Debug.Log("No path from " + fromPos + " to " + toPos);
            }
            else //if path can be found, add path to connected area on pathMap
            {
                connected = true;
                for (int i = 0; i < mapWidth; i++)
                    for (int j = 0; j < mapHeight; j++)
                    {
                        if (pathMap[i, j] == 'p')
                            pathMap[i, j] = 'c';
                    }
            }
        }
        if (connected) //if room is connected, add room to connected area on pathMap 
        {
            for (int i = fromRoom.X + 1; i < fromRoom.X + fromRoom.Width - 1; i++)
                for (int j = fromRoom.Y + 1; j < fromRoom.Y + fromRoom.Height - 1; j++)
                {
                    pathMap[i, j] = 'c';
                }
            fromRoom.Connected = true;
        }
        //debug: else: undo room?
    }


    private bool FindPath(Vector3Int previousPos, Vector3Int currentPos, Vector3Int goalPos, int xOrY)
    {
        Debug.Log("try path from " + currentPos + " to " + goalPos);
        if(currentPos == goalPos)
        {
            Debug.Log("goal reached");
            return true;
        }
        if (currentPos.x < 0 || currentPos.x >= mapWidth || currentPos.y < 0 || currentPos.y >= mapHeight)
        {
            Debug.Log("abort, path cannot go outside of tileMap");
            return false;
        }
        if (pathMap[currentPos.x, currentPos.y] == 'n' || pathMap[currentPos.x, currentPos.y] == 'p')
        {
            Debug.Log("abort, this tile has already been tried out");
            return false;
        }
        if (map[currentPos.x, currentPos.y] == 'w' && map[previousPos.x, previousPos.y] == 'w')
        {
            Debug.Log("abort, the path cannot go through two or more consecutive walls");
            return false;
        }
        if (map[currentPos.x, currentPos.y] == 'w' && map[previousPos.x, previousPos.y] == 'd'
            || map[currentPos.x, currentPos.y] == 'd' && map[previousPos.x, previousPos.y] == 'w')
        {
            Debug.Log("abort, no two consecutive doors in path allowed");
            return false;
        }
        if (pathMap[currentPos.x, currentPos.y] == 'c')
        {
            Debug.Log("connected area reached");
            return true;
        }
        if (currentPos.x == goalPos.x && xOrY == 0)
            xOrY = 1;
        if (currentPos.y == goalPos.y && xOrY == 1)
            xOrY = 0;

        pathMap[currentPos.x, currentPos.y] = 'p';
        
        int xDirection;

        if (goalPos.x - currentPos.x > 0 || goalPos.x - currentPos.x < 0)
            xDirection = (goalPos.x - currentPos.x) / Mathf.Abs(goalPos.x - currentPos.x);
        else
            xDirection = 0;
        
        int yDirection;

        if (goalPos.y - currentPos.y > 0 || goalPos.y - currentPos.y < 0)
            yDirection = (goalPos.y - currentPos.y) / Mathf.Abs(goalPos.y - currentPos.y);
        else
            yDirection = 0;

        Vector3Int nextPos;

        if(xOrY == 0)
            nextPos = new Vector3Int(currentPos.x + xDirection, currentPos.y, z);
        else
            nextPos = new Vector3Int(currentPos.x, currentPos.y + yDirection, z);

        Debug.Log("from " + currentPos + " go to " + nextPos);
        if (FindPath(currentPos, nextPos, goalPos, xOrY) && nextPos != previousPos)
        {
            PaintPath(currentPos, previousPos);
            return true;
        }

        if (map[currentPos.x, currentPos.y] != 'w')
        {
            if (xOrY == 0)
            {
                xOrY = 1;
                if(yDirection == 0)
                {
                    yDirection = 1;
                }
                nextPos = new Vector3Int(currentPos.x, currentPos.y + yDirection, z);
            }
            else
            {
                xOrY = 0;
                if (xDirection == 0)
                    xDirection = 1;
                nextPos = new Vector3Int(currentPos.x + xDirection, currentPos.y, z);
            }

            Debug.Log("from " + currentPos + " go to " + nextPos);
            if (FindPath(currentPos, nextPos, goalPos, xOrY) && nextPos != previousPos)
            {
                PaintPath(currentPos, previousPos);
                return true;
            }

            if (xOrY == 0)
                nextPos = new Vector3Int(currentPos.x - xDirection, currentPos.y, z);
            else
                nextPos = new Vector3Int(currentPos.x, currentPos.y - yDirection, z);

            Debug.Log("from " + currentPos + " go to " + nextPos);
            if (FindPath(currentPos, nextPos, goalPos, xOrY) && nextPos != previousPos)
            {
                PaintPath(currentPos, previousPos);
                return true;
            }
        }

        pathMap[currentPos.x, currentPos.y] = 'n';
        Debug.Log("pathMap[" + currentPos.x + "," + currentPos.y + "] = " + pathMap[currentPos.x, currentPos.y]);
        Debug.Log("abort, every direction from " + currentPos + " is impossible");
        return false;
    }

    private void PaintPath(Vector3Int currentPos, Vector3Int previousPos)
    {
        if (map[currentPos.x, currentPos.y] == 'w')
        {
            Debug.Log("currentPosition " + currentPos + " is a wall tile, will be turned into door");
            if (currentPos.x == previousPos.x || currentPos.y == previousPos.y)
            {
                map[currentPos.x, currentPos.y] = 'd';
            }
        }
        if (map[currentPos.x, currentPos.y] == 'o')
        {
            Debug.Log("currentPosition " + currentPos + " is empty, will be turned into floor");
            map[currentPos.x, currentPos.y] = 'f';
        }
    }

    private Vector3Int PointInRoom(Room r)
    {
        int randomX = UnityEngine.Random.Range(r.X + 1, r.X + r.Width - 1);
        int randomY = UnityEngine.Random.Range(r.Y + 1, r.Y + r.Height - 1);

        return new Vector3Int(randomX, randomY, z);
    }

    private bool IfAbleBuildRoom(int width, int height, int x, int y)
    {
        bool moved = false;
        if (x + width > mapWidth || y + height > mapHeight)
        {
            return false;
        }

        for (int i = x; i < x + width; i++)
            for (int j = y; j < y + height; j++)
            {
                if (map[i, j] == 'f') //if trying to build on a floor tile
                {
                    if (roomOrPath[i, j] == 'r') //if trying to build on an existing room
                        return false;
                    else if ((i == x && (j == y || j == y + height - 1))
                        || (i == x + width - 1 && (j == y || j == y + height - 1))) //corner of new room
                        return false;
                    else if (((i == x || i == x + width - 1) && !(j == y || j == y + height - 1)) 
                        ||
                        (!(i == x || i == x + width - 1) && (j == y || j == y + height - 1))) //if building wall exluding corners
                    {
                        if (map[i - 1, j] == 'd' || map[i + 1, j] == 'd'
                            || map[i, j - 1] == 'd' || map[i, j + 1] == 'd') 
                            //if any neighbouring tile is already a door, abort,
                            //because path should not be severed
                            //and also there should be no doors right next to each other
                            return false;
                        else
                            map[i, j] = 'd'; //debug: if room ends up not getting built, this should not be a door?
                    }
                }
                if (map[i, j] == 'w')
                    if (i != x && i != x + width - 1 && j != y && j != y + height - 1) //if trying to build floor instead of wall
                        return false;
                if(map[i,j] == 0) //if trying to build on empty tile but neighbouring tile is wall, move build location
                {
                    if (i == x && map[i - 1, j] == 'w')
                    {
                        if (!roomBuildMoved)
                        {
                            Debug.Log("trying left wall at " + new Vector3Int(i, j, z)
                                + " and finding neighbouring existing wall at " + new Vector3Int(i - 1, j, z)
                                + ", so move building site to the left to overlap wall with other room");
                            randX--;
                            roomBuildMoved = true;
                            moved = true;
                        }
                        else
                            return false;
                    }
                    if (i == x + width - 1 && map[i + 1, j] == 'w')
                    {
                        if (!roomBuildMoved)
                        {
                            Debug.Log("trying right wall at " + new Vector3Int(i, j, z)
                                + " and finding neighbouring existing wall at " + new Vector3Int(i + 1, j, z)
                                + ", so move building site to the right to overlap wall with other room");
                            randX++;
                            roomBuildMoved = true;
                            moved = true;
                        }
                        else
                            return false;
                    }
                    if (j == y && map[i, j - 1] == 'w')
                    {
                        if (!roomBuildMoved)
                        {
                            Debug.Log("trying lower wall at " + new Vector3Int(i, j, z)
                                + " and finding neighbouring existing wall at " + new Vector3Int(i, j - 1, z)
                                + ", so move building site down to overlap wall with other room");
                            randY--;
                            roomBuildMoved = true;
                            moved = true;
                        }
                        else
                            return false;
                    }
                    if (j == y + height - 1 && map[i, j + 1] == 'w')
                    {
                        if (!roomBuildMoved)
                        {
                            Debug.Log("trying upper wall at " + new Vector3Int(i, j, z)
                                + " and finding neighbouring existing wall at " + new Vector3Int(i, j + 1, z)
                                + ", so move building site up to overlap wall with other room");
                            randY++;
                            roomBuildMoved = true;
                            moved = true;
                        }
                        else
                            return false;
                    }
                    if (moved)
                    {
                        if (IfAbleBuildRoom(randWidth, randHeight, randX, randY))
                            return true;
                        else
                            return false;
                    }
                }
            }
        BuildRoom(width, height, x, y);
        return true;
    }

    private void BuildRoom(int width, int height, int x, int y)
    {
        Debug.Log("building room from " + new Vector3Int(x, y, z) + " to " + new Vector3Int(x + width - 1, y + height - 1,z));
        for (int i = x; i <  x + width; i++)
            for (int j = y; j < y + height; j++)
            {
                if (map[i, j] == 'd')
                {
                    Debug.Log("building room door at " + new Vector3Int(i, j, z));
                }
                else if (i == x || i == x + width - 1 || j == y || j == y + height - 1)
                {
                    Debug.Log("building wall at " + new Vector3Int(i, j, z));
                    map[i, j] = 'w';
                }
                else
                {
                    Debug.Log("building floor at " + new Vector3Int(i, j, z));
                    map[i, j] = 'f';
                }
                roomOrPath[i, j] = 'r';
            }
        Debug.Log("room from " + new Vector3Int(x, y, z) + " to " + new Vector3Int(x + width - 1, y + height - 1, z) + " is built");
    }
}
