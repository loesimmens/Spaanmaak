using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private EntityData npcData;
    public MapData mapData;

    // Start is called before the first frame update
    void Start()
    {
        npcData = new EntityData();
        npcData.InitialState = State.idle;
        npcData.CurrentState = npcData.InitialState;

        transform.position = new Vector3(mapData.Lddr.DownPos.x - 0.5f, mapData.Lddr.DownPos.y + 0.5f, mapData.Lddr.DownPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
