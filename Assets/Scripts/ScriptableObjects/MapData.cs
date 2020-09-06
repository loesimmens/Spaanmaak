using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    [SerializeField]
    private char[,,] map;
    [SerializeField]
    private Tilemap tileMap;
    [SerializeField]
    private List<Room> rooms;
    [SerializeField]
    private int nRooms;
    [SerializeField]
    private Ladder lddr;
    [SerializeField]
    private bool underground;

    public char[,] Map
    { get; set; }

    public Tilemap TileMap
    { get; set; }

    public List<Room> Rooms
    { get; set; }

    public int NRooms
    { get; set; }

    public Ladder Lddr
    {
        get; set;
    }

    public bool Underground
    { get; set; }
}
