using UnityEngine;
using UnityEditor;

/*
    to do:
        - set position to center
        - implement size and gap setting
        - add preset feature
        - add manual grid editing
*/

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

    [MenuItem("Window/Grid Generator")]
    public static void Init()
    {    
        GridGeneratorWindow window = GetWindow<GridGeneratorWindow>("Grid Generator");
        window.minSize = new Vector2(240, 0);
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

        EditorGUIUtility.labelWidth = 72;

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

        EditorGUILayout.LabelField("Nodes", EditorStyles.boldLabel);
        _scrollView = EditorGUILayout.BeginScrollView(_scrollView, GUILayout.Width(position.width), GUILayout.Height(320));
            
            EditorGUILayout.PropertyField(_nodes);

        EditorGUILayout.EndScrollView();

        if(GUILayout.Button("Generate"))
        {
            GenerateGrid();
        }

        _serializedObject.ApplyModifiedProperties();
    }

    private void GenerateGrid()
    {
        Transform parentObject = new GameObject(_gridName.stringValue).transform;

        for (int row = 0; row < _row.intValue; row++)
        {
            for (int column = 0; column < _column.intValue; column++)
            {
                int rngIndex = Random.Range(0, _nodes.arraySize);
                Vector3 pos = new Vector3(row, 0, column);

                GameObject node = (GameObject)Instantiate(_nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Object").objectReferenceValue, 
                                pos,
                                Quaternion.identity,
                                parentObject);
                node.name = _nodes.GetArrayElementAtIndex(rngIndex).FindPropertyRelative("Name").stringValue;
            }
        }
    }
}
