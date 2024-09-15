using UnityEngine;
using UnityEditor;

public class GridGeneratorWindow : EditorWindow
{
    private static GridGeneratorData _data;
    private SerializedObject _serializedObject;

    private SerializedProperty _gridName;
    private SerializedProperty _row;
    private SerializedProperty _column;
    private SerializedProperty _size;
    private SerializedProperty _gap;
    private SerializedProperty _nodes;

    private SerializedProperty _nodesFoldout;
    private SerializedProperty _overwriteExisted;

    [MenuItem("Window/Grid Tool/Generator")]
    public static void Init()
    {    
        GridGeneratorWindow window = GetWindow<GridGeneratorWindow>("Grid Generator");
        window.minSize = new Vector2(360, 480);
        window.Show();
    }

    [InitializeOnLoadMethod]
    public static void OnLoad()
    {
        if(!_data)
        {
            // check if there is setting data instance
            _data = AssetDatabase.LoadAssetAtPath<GridGeneratorData>("Assets/Grid Generator/Editor/GridGeneratorData.asset");
            if(_data) return;

            // otherwise create and reference new setting data
            _data = CreateInstance<GridGeneratorData>();
            AssetDatabase.CreateAsset(_data, "Assets/Grid Generator/Editor/GridGeneratorData.asset");
            AssetDatabase.Refresh();
        }
    }

    Vector2 _scrollView;
    private void OnGUI()
    {
        _serializedObject = new SerializedObject(_data);
        _serializedObject.Update();

        _gridName = _serializedObject.FindProperty("gridName");
        _row = _serializedObject.FindProperty("row");
        _column = _serializedObject.FindProperty("column");
        _size = _serializedObject.FindProperty("size");
        _gap = _serializedObject.FindProperty("gap");
        _nodes = _serializedObject.FindProperty("nodes");
        _nodesFoldout = _serializedObject.FindProperty("nodeFoldout");
        _overwriteExisted = _serializedObject.FindProperty("overwriteExisted");

        EditorGUIUtility.labelWidth = 64;

        // draw general setting
        EditorGUILayout.LabelField("Grid Setting", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_gridName);
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_row);
            EditorGUILayout.PropertyField(_column);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(_size);
            EditorGUILayout.PropertyField(_gap);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // draw nodes
        _nodesFoldout.boolValue = EditorGUILayout.Foldout(_nodesFoldout.boolValue, new GUIContent("Node List"));
        if(_nodesFoldout.boolValue)
        {
            _scrollView = EditorGUILayout.BeginScrollView(_scrollView, GUILayout.Width(position.width), GUILayout.Height(position.height-230));
                if(_nodes.arraySize == 0)
                    EditorGUILayout.HelpBox("Currently there is no node available. Please add node by pressing [Add] button bellow.", MessageType.Warning);
                for(int i = 0; i < _nodes.arraySize; i++)
                    DrawNode(i);
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space(10);

        if(CheckNonOneRate())
            EditorGUILayout.HelpBox("CANNOT GENERATE GRID. There is no node with [Rate = 1] detected. Please set a node's Rate value to 1 at least one node.", MessageType.Error);

        // draw buttons
        EditorGUIUtility.labelWidth = 136;
        EditorGUILayout.PropertyField(_overwriteExisted, new GUIContent("Overwrite existed Grid?"));
        EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Remove All"))
                _nodes.ClearArray();
            if (GUILayout.Button("Add"))
                AddNode();
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Generate"))
            GenerateGrid();

        _serializedObject.ApplyModifiedProperties();
    }

    private bool CheckNonOneRate()
    {
        for (int i = 0; i < _nodes.arraySize; i++)
        {
            if(_nodes.GetArrayElementAtIndex(i).FindPropertyRelative("rate").floatValue == 1)
                return false;
        }
        return true;
    }

    private void DrawNode(int index)
    {
        if(_nodes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue != "")
            EditorGUILayout.LabelField(_nodes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue, EditorStyles.label);
        else
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

        _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue = EditorGUILayout.TextField("Name", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue);
        _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("nodeObject").objectReferenceValue = EditorGUILayout.ObjectField("Object", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("nodeObject").objectReferenceValue, typeof(GameObject), true);
        EditorGUILayout.BeginHorizontal();
            _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("size").floatValue = EditorGUILayout.FloatField("Size", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("size").floatValue);
            _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("rate").floatValue = EditorGUILayout.Slider("Rate", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("rate").floatValue, 0, 1);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Remove"))
                _nodes.DeleteArrayElementAtIndex(index);
            if(GUILayout.Button("Duplicate"))
                DuplicateNode(index);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    private void AddNode()
    {
        _nodes.arraySize++;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("name").stringValue = "";
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("nodeObject").objectReferenceValue = null;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("size").floatValue = 1;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("rate").floatValue = 1;
    }

    private void DuplicateNode(int index)
    {
        _nodes.arraySize++;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("name").stringValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("nodeObject").objectReferenceValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("nodeObject").objectReferenceValue;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("size").floatValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("size").floatValue;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("rate").floatValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("rate").floatValue;
    }

    private void GenerateGrid()
    {
        // to prevent infinity loop when generating grid
        if(CheckNonOneRate()) return;

        if(_overwriteExisted.boolValue && GameObject.Find(_gridName.stringValue))
            DestroyImmediate(GameObject.Find(_gridName.stringValue).gameObject);

        Transform parentObject = new GameObject(_gridName.stringValue).transform;

        for (int row = 0; row < _row.intValue; row++)
        {
            for (int column = 0; column < _column.intValue; column++)
            {
                int rngIndex = Random.Range(0, _nodes.arraySize);
                float rngRate = (float)Random.Range(0, 100) / 100;

                while (rngRate > _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("rate").floatValue)
                    rngIndex = Random.Range(0, _nodes.arraySize);

                Vector3 pos = new Vector3(((float)(-1 + _row.intValue)/2 - row) * _size.floatValue,
                                0,
                                ((float)(-1 + _column.intValue)/2 - column) * _size.floatValue);

                GameObject node = (GameObject)Instantiate(_nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("nodeObject").objectReferenceValue, 
                                pos,
                                Quaternion.identity,
                                parentObject);
                if(_nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("name").stringValue != "")
                    node.name = _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("name").stringValue;
                else
                    node.name = "Node";

                node.transform.localScale = Vector3.one * _size.floatValue * _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("size").floatValue;
            }
        }
    }
}
