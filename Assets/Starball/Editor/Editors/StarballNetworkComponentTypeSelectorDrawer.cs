namespace Izzo.Starball.Editor
{
    using Izzo.Editor;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;


    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary> A custom drawer for NetworkGameObjectSelector </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [CustomEditor( typeof( StarballNetworkComponentTypeSelector ) )]

    public class StarballNetworkComponentTypeSelectorDrawer : Editor
    {
        ReorderableList objectList;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets up drawer references.            </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        protected void OnEnable()
        {
            objectList = EditorUtil.CreateReorderableList( serializedObject,
                                                           "_objectsToStrip",
                                                           "Objects to strip" );
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>  Sets up drawer references.            </summary>
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            objectList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            if( GUILayout.Button( "Repopulate Lists" ) )
            {
                var componentTypeSelector =
                serializedObject.targetObject as
                StarballNetworkComponentTypeSelector;

                componentTypeSelector.UpdateComponentSelector();
            }
        }
    }
}