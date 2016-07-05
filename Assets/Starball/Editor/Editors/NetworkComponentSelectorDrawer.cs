using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Izzo.Networking;

//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/// <summary> A custom drawe for NetworkComponentSelector </summary>
//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[CustomEditor( typeof( NetworkComponentSelector ) )]

public class NetworkComponentSelectorDrawer : Editor
{
    private ReorderableList clientList;
    private ReorderableList serverList;
    private bool showClientList = true;
    private bool showServerList = true;

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    /// <summary>  Sets up drawer references.            </summary>
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    protected void OnEnable()
    {
        clientList = CreateReorderableList( serializedObject,
                                            "_clientOnlyComponents",
                                            "Client Only Components" );
        serverList = CreateReorderableList( serializedObject,
                                            "_serverOnlyComponents",
                                            "Server Only Components" );
    }

    //=============================================================
    /// <summary>  Does the drawing of the component.    </summary>
    //==================================
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

    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    /// <summary>  An utility function 
    ///            to create a ReorderableList.          </summary>
    /// <param name="serializedObject">
    ///     An serialized object whose property will be used
    ///     as the data of this list.                      </param>
    /// <param name="propertyName">
    ///     The name of the property of type IList that
    ///     will be used to fill the elements of the list. </param>
    /// <param name="label">
    ///     The label to be used as a header of the list.  </param>
    /// <returns>  A new constructed ReorderableList.    </returns>
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private static ReorderableList
    CreateReorderableList( SerializedObject serializedObject,
                           string propertyName,
                           string label )
    {
        var listProperty = serializedObject.FindProperty( propertyName );
        var list = new ReorderableList( serializedObject, listProperty );

        // List Header - draws the label 
        list.drawHeaderCallback = 
            ( Rect rect ) =>
        {
            EditorGUI.LabelField( rect, label );
        };

        // List Elements - draws each individual item
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
