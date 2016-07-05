using UnityEditor;
using UnityEngine;

//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
/// <summary>  Custom drawer for flags enum properties. </summary>
//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
[CustomPropertyDrawer( typeof( EnumFlagsAttribute ) )]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    //=============================================================
    /// <summary>  Draws and updates the state 
    ///            of a serialized enum property.        </summary>
    //==================================
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
    }
}