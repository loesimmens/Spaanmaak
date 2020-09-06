using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.RestService;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public EntityData playerData;

    public MapData mapData;
    private readonly float moveSpeed = 5f;
    public Transform movePoint;

    public LayerMask whatStopsMovement;


    // Start is called before the first frame update
    void Start()
    {
        mapData.Underground = false;
        
        playerData.CurrentState = playerData.InitialState;
        
        transform.position = new Vector3(mapData.Lddr.DownPos.x + 1.5f, mapData.Lddr.DownPos.y + 0.5f, mapData.Lddr.DownPos.z);
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
        {
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f)
            {
                if (mapData.Map[(int)(movePoint.position.x - 0.5f + Input.GetAxisRaw("Horizontal")), (int)(movePoint.position.y - 0.5f)] != 'w')
                {
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                }
                
                if (new Vector3((int)(movePoint.position.x - 0.5f), (int)(movePoint.position.y - 0.5f), 0) == mapData.Lddr.DownPos)
                {
                    movePoint.position += new Vector3(0f, 0f, -1f);
                    mapData.Underground = false;
                }

                else if (new Vector3((int)(movePoint.position.x - 0.5f), (int)(movePoint.position.y - 0.5f), 0) == mapData.Lddr.UpPos)
                {
                    movePoint.position += new Vector3(0f, 0f, +1f);
                    mapData.Underground = true;
                }
            }

            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f)
            {
                if (mapData.Map[(int)(movePoint.position.x - 0.5f), (int)(movePoint.position.y - 0.5f + Input.GetAxisRaw("Vertical"))] != 'w')
                {
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                }
                
                if (new Vector3((int)(movePoint.position.x - 0.5f), (int)(movePoint.position.y - 0.5f), 0) == mapData.Lddr.DownPos)
                {
                    movePoint.position += new Vector3(0f, 0f, -1f);
                    mapData.Underground = false;
                }

                else if (new Vector3((int)(movePoint.position.x - 0.5f), (int)(movePoint.position.y - 0.5f), 0) == mapData.Lddr.UpPos)
                {
                    movePoint.position += new Vector3(0f, 0f, +1f);
                    mapData.Underground = true;
                }
            }

            
        }
    }
}
