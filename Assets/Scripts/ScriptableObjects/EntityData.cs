using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    idle,
    walk,
    interact,
    attack
}

[CreateAssetMenu]
public class EntityData : ScriptableObject
{
    [SerializeField]
    private State initialState;
    private State currentState;

    public State InitialState
    { get; set; }
    
    public State CurrentState
    { get; set; }
}
