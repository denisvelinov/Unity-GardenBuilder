using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }

    public Tile tileUnknown;
    public Tile dirt;
    public Tile water;
    public Tile stone;
    public Tile mycelium;
    public Tile rootActive;
    public Tile rootInactive;
    public Tile seedling;
    public Tile tree;
    public Tile mushroom;
    public Tile fern;

    private void Awake()
    {
        this.tilemap = GetComponent<Tilemap>();
    }

    public void SetSeedling() 
    {
        Vector3Int seedlingPossition = new Vector3Int(5,12);
        tilemap.SetTile(seedlingPossition, seedling);
    }

    public void SetTree()
    {
        Vector3Int treePossition = new Vector3Int(5, 12);
        tilemap.SetTile(treePossition, tree);
    }

    public void SetMushroom()
    {
        Vector3Int mushroomPossition = new Vector3Int(0, 12);
        tilemap.SetTile(mushroomPossition, mushroom);
    }

    public void SetFern()
    {
        Vector3Int fernPossition = new Vector3Int(-4, 12);
        tilemap.SetTile(fernPossition, fern);
    }

    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell) 
    {
        if (!cell.revealed)
        {
            return tileUnknown;
        }
        else
        {
            return GetrevealedTile(cell);
        }
    }

    private Tile GetrevealedTile(Cell cell) 
    {
        switch (cell.type)
        {
            case Cell.Type.Dirt: return dirt;
            case Cell.Type.Water: return water;
            case Cell.Type.Stone: return stone;
            case Cell.Type.Mycelium: return mycelium;
            case Cell.Type.RootActive: return rootActive;
            case Cell.Type.RootInactive: return rootInactive;
            default: return null;
        }
    }
}
