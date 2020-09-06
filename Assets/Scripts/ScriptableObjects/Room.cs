using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

[CreateAssetMenu]
public class Room : ScriptableObject
{
    private int width;
    private int height;
    private int x;
    private int y;
    private bool startingRoom;
    private bool endingRoom;
    private bool connected;
    public void Init(int width, int height, int x, int y
        , bool startingRoom, bool endingRoom)
    {
        this.width = width;
        this.height = height;
        this.x = x;
        this.y = y;
        this.startingRoom = startingRoom;
        this.endingRoom = endingRoom;
    }

    public int Width
    { get; set; }

    public int Height
    { get; set; }

    public int X
    { get; set; }

    public int Y
    { get; set; }

    public bool StartingRoom
    { get; set; }

    public bool EndingRoom
    { get; set; }

    public bool Connected
    { get; set; }
}
