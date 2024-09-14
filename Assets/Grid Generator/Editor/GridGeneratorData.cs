using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridNode
{
    public string Name;
    public GameObject Object;
    public float Size;
}

public class GridGeneratorData : ScriptableObject
{
    public string GridName = "grid_name";
    public int Row = 6;
    public int Column = 6;
    public float Size = 1;
    public float Gap = 0;
    public List<GridNode> Nodes;
}
