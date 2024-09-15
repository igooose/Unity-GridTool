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

        _gridName = _serializedObject.FindProperty("GridName");
        _row = _serializedObject.FindProperty("Row");
        _column = _serializedObject.FindProperty("Column");
        _size = _serializedObject.FindProperty("Size");
        _gap = _serializedObject.FindProperty("Gap");
        _nodes = _serializedObject.FindProperty("Nodes");

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
        EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel);
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView, GUILayout.Width(position.width), GUILayout.Height(position.height-160));
            if(_nodes.arraySize == 0)
                EditorGUILayout.HelpBox("Currently there is no node available. Please add node by pressing [Add] button bellow.", MessageType.Warning);
            for(int i = 0; i < _nodes.arraySize; i++)
                DrawNode(i);
        EditorGUILayout.EndScrollView();

        // draw buttons
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

    private void DrawNode(int index)
    {
        if(_nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue != "")
            EditorGUILayout.LabelField(_nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue, EditorStyles.boldLabel);
        else
            EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

        _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue = EditorGUILayout.TextField("Name", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue);
        EditorGUILayout.BeginHorizontal();
            _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Object").objectReferenceValue = EditorGUILayout.ObjectField("Object", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Object").objectReferenceValue, typeof(GameObject), true);
            _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Size").floatValue = EditorGUILayout.FloatField("Size", _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Size").floatValue);
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
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Name").stringValue = "";
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Object").objectReferenceValue = null;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Size").floatValue = 1;
    }

    private void DuplicateNode(int index)
    {
        _nodes.arraySize++;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Name").stringValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Name").stringValue;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Object").objectReferenceValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Object").objectReferenceValue;
        _nodes.GetArrayElementAtIndex(_nodes.arraySize - 1).FindPropertyRelative("Size").floatValue = _nodes.GetArrayElementAtIndex(index).FindPropertyRelative("Size").floatValue;
    }

    private void GenerateGrid()
    {
        Transform parentObject = new GameObject(_gridName.stringValue).transform;

        for (int row = 0; row < _row.intValue; row++)
        {
            for (int column = 0; column < _column.intValue; column++)
            {
                int rngIndex = Random.Range(0, _nodes.arraySize);
                Vector3 pos = new Vector3(((float)(-1 + _row.intValue)/2 - row) * _size.floatValue,
                                0,
                                ((float)(-1 + _column.intValue)/2 - column) * _size.floatValue);

                GameObject node = (GameObject)Instantiate(_nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Object").objectReferenceValue, 
                                pos,
                                Quaternion.identity,
                                parentObject);
                if(_nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Name").stringValue != "")
                    node.name = _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Name").stringValue;
                else
                    node.name = "Node";

                node.transform.localScale = Vector3.one * _size.floatValue * _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Size").floatValue;
            }
        }
    }
}
