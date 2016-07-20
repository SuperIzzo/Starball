namespace Izzo.Networking.Editor
{
    using Izzo.Editor;
    using UnityEditor;
    using UnityEditorInternal;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// <summary> A custom drawer for NetworkGameObjectSelector </summary>
    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    [CustomEditor( typeof( NetworkGameObjectSelector ) )]

    public class NetworkGameObjectSelectorDrawer : Editor
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
                                                "_clientOnlyObjects",
                                                "Client Only Objects" );
            serverList = EditorUtil.CreateReorderableList( serializedObject,
                                                "_serverOnlyObjects",
                                                "Server Only Objects" );
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
    }
}