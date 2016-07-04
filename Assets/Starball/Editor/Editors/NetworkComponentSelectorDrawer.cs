using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Izzo.Networking;

[CustomEditor( typeof( NetworkComponentSelector ) )]
public class NetworkComponentSelectorDrawer : Editor
{
    private ReorderableList clientList;
    private ReorderableList serverList;
    private bool showClientList = true;
    private bool showServerList = true;

    protected void OnEnable()
    {
        clientList = CreateReorderableList( serializedObject,
                                            "_clientOnlyComponents", 
                                            "Client Only Components" );
        serverList = CreateReorderableList( serializedObject,
                                            "_serverOnlyComponents",
                                            "Server Only Components" );
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showClientList = EditorGUILayout.Foldout( showClientList, "Client" );
        if( showClientList )
        {
            clientList.DoLayoutList();
        }

        showServerList = EditorGUILayout.Foldout( showServerList, "Server" );
        if( showServerList )
        {
            serverList.DoLayoutList();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static ReorderableList
    CreateReorderableList( SerializedObject serializedObject,
                           string propertyName,
                           string label )
    {
        var listProperty = serializedObject.FindProperty( propertyName );
        var list = new ReorderableList( serializedObject, listProperty );

        list.drawHeaderCallback = ( Rect rect ) =>
        {
            EditorGUI.LabelField( rect, label );
        };

        list.drawElementCallback =
            ( Rect rect, int index, bool isActive, bool isFocused ) =>
            {
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;
                var element = listProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField( rect, element, GUIContent.none );
            };

        return list;
    }
}
