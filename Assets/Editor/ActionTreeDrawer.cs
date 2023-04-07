using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Character.ActionTree))]
public class ActionTreeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if(GUI.Button(position, "Open Action Editor"))
        {
            Debug.Log("Editor opening");
        }
    }
}
