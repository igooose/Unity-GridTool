using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridNode
{
    public string Name;
    public GameObject Object;
    public int Size;
}

public class GridGeneratorData : ScriptableObject
{
    public string GridName = "grid_name";
    public int Row = 6;
    public int Column = 6;
    public int Size = 1;
    public int Gap = 0;
    public List<GridNode> Nodes;
}
