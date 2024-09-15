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
    public string gridName = "grid_name";
    public int row = 6;
    public int column = 6;
    public float size = 1;
    public float gap = 0;
    public List<GridNode> nodes;

    public bool nodeFoldout;
    public bool overwriteExisted;
}
