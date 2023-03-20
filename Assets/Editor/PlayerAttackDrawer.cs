using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PlayerCharacter.PlayerAction))]
public class PlayerAttackDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty chain = property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.chain));
        SerializedProperty attack = property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.name));
        SerializedProperty initialState = property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.initialState));
        EditorGUI.BeginProperty(position, label, property);

        string code = "";
        for (int counter = 0; counter < chain.arraySize; counter++)
        {
            var a = chain.GetArrayElementAtIndex(counter).FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.PlayerActionItem.actionCommand)).enumValueIndex;
            var a2 = (PlayerCharacter.PlayerAction.ActionCommand)a;
            if (a2 == PlayerCharacter.PlayerAction.ActionCommand.FastAttack) code += "F";
            else if (a2 == PlayerCharacter.PlayerAction.ActionCommand.SlowAttack) code += "S";
            else if (a2 == PlayerCharacter.PlayerAction.ActionCommand.Jump) code += "J";
            else if (a2 == PlayerCharacter.PlayerAction.ActionCommand.SpecialAttack) code += "P";
            else if (a2 == PlayerCharacter.PlayerAction.ActionCommand.Dodge) code += "D";
            if (counter != chain.arraySize - 1) code += ",";            
        }

        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none)), property.isExpanded, $"{attack.stringValue}: {code}", true);
        if (property.isExpanded)
        {
            float y = position.y;
            y += EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);

            float height = EditorGUI.GetPropertyHeight(attack);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), attack);
            y += height;

            height = EditorGUI.GetPropertyHeight(initialState);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), initialState);
            y += height;

            height = EditorGUI.GetPropertyHeight(chain);
            EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), chain);
        }
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float h = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, GUIContent.none);
        if (property.isExpanded)
        {
            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.name)));
            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.initialState)));
            h += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.chain)));
        }
        return h;
    }
}
