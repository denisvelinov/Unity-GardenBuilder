using System;
using UnityEngine;

public struct Cell
{
    public enum Type 
    {
        Invalid,
        Dirt,
        Water,
        Stone,
        Mycelium,
        RootActive,
        RootInactive,
    }

    public Vector3Int position;
    public Type type;
    public bool revealed;
}
