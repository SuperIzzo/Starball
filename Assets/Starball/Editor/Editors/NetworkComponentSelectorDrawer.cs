namespace Izzo.Networking.Editor
{
    using Izzo.Editor;
    using UnityEditor;
    using UnityEditorInternal;

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
            clientList = EditorUtil.CreateReorderableList( serializedObject,
                                                "_clientOnlyComponents",
                                                "Client Only Components" );
            serverList = EditorUtil.CreateReorderableList( serializedObject,
                                                "_serverOnlyComponents",
                                                "Server Only Components" );
        }

        //=============================================================
        /// <summary>  Does the drawing of the component.    </summary>
        //==================================
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showClientList = 
                EditorGUILayout.Foldout( showClientList, "Client" );

            if( showClientList )
            {
                clientList.DoLayoutList();
            }

            showServerList = 
                EditorGUILayout.Foldout( showServerList, "Server" );

            if( showServerList )
            {
                serverList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}