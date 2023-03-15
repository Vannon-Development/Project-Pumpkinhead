using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PlayerCharacter.PlayerAction.PlayerActionItem))]
public class PlayerAttackActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect[] positions = {
            new Rect(position.x, position.y, position.width * 0.3f, position.height),
            new Rect(position.x + position.width * 0.64f, position.y, position.width * 0.2f, position.height),
            new Rect(position.x + position.width * 0.86f, position.y, position.width * 0.1f, position.height)
        };

        EditorGUI.PropertyField(positions[0], property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.PlayerActionItem.action)), GUIContent.none);
        EditorGUI.PropertyField(positions[1], property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.PlayerActionItem.animationIndex)), GUIContent.none);
        EditorGUI.PropertyField(positions[2], property.FindPropertyRelative(nameof(PlayerCharacter.PlayerAction.PlayerActionItem.requiresHit)), GUIContent.none);

        EditorGUI.EndProperty();
    }
}
