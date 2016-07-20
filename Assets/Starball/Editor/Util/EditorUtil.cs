namespace Izzo.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;    

    public static class EditorUtil
    {
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
        public static ReorderableList
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
}