using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map01 : Map
{
    [SerializeField] private List<Vector3> toReplaceTiles;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private Tilemap tilemap;
    string mapName = "Map01";

    /*override public Vector3[] GetPositions()
    {
        Debug.Log("Map, GetPositions : PositionsLength = " + positions.Length);
        return positions;
    }*/
    override public string GetName()
    {
        Debug.Log("Map, GetName : MapName = " + mapName);
        return mapName;
    }
    override public List<string> GetTilesName()
    {
        var names = new List<string>();
        foreach (TileBase tile in tiles)
        {
            names.Add(tile.name);
        }
        return names;

    }

    override public bool FindInNames(string name)
    {
        foreach(string tile in GetTilesName())
        {
            if (tile == name) return true;
        }

        return false;
    }

    public TileBase GetTileByNumber(int nb)
    {
        Debug.Log("number tile : " + nb);
        Debug.Log("count tile : " + tiles.Count);
        if (nb < 0 || nb >= tiles.Count) return null;
        return tiles[nb];
    }
    public TileBase GetTileByName(string name)
    {
        foreach (TileBase tile in tiles)
        {
            if (tile.name == name) return tile;
        }
        return null;
    }
    public int GetIndexByName(string name)
    {
        foreach (TileBase tile in tiles)
        {
            if (tile.name == name) return FindTileIndex(tile);
        }
        return FindTileIndex(null);
    }

    override public TileBase GetNextByName(string name)
        { 
            foreach (TileBase tile in tiles)
            {
                if (tile.name == name) return GetTileByNumber( FindTileIndex(tile) + 1 );
            }
            return GetTileByNumber(-1);
        }

    override public int FindTileIndex(TileBase tile)
    {
        if (tile == null) return -10;
        return tiles.IndexOf(tile);

    }
    override public int GetListCount()
    {
        return tiles.Count;
    }
    override public void StoreVect(Vector3 vect)
    {
        toReplaceTiles.Add(vect);
    }
    override public void ReplaceTiles()
    {
        foreach(Vector3 vect in toReplaceTiles)
        {
            tilemap.SetTile(tilemap.WorldToCell(vect), GetTileByNumber(0));
        }
    }
}
