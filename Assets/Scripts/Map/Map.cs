using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

abstract public class Map : MonoBehaviour
{
    //Vector3[] positions;
    //string mapName;

    //abstract public Vector3[] GetPositions();
    abstract public string GetName();
    abstract public int FindTileIndex(TileBase tile);
    abstract public int GetListCount();
    abstract public TileBase GetNextByName(string name);
    abstract public void StoreVect(Vector3 vect);
    abstract public void ReplaceTiles();
    abstract public List<string> GetTilesName();
    abstract public bool FindInNames(string name);


}

