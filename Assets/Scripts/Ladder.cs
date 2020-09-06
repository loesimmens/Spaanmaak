using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : ScriptableObject
{
    private Vector3Int upPos;
    private Vector3Int downPos;

    public Vector3Int UpPos
    { get; set; }

    public Vector3Int DownPos
    { get; set; }
}
