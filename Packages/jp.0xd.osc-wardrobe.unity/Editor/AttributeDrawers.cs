using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using called_D.OscWardrobe.Unity;

namespace called_D.OscWardrobe.Unity.Editor
{
    [CustomPropertyDrawer(typeof(AvatarAliasAttribute))]
    public class AvatarAliasDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var aliases = Utils.Aliases;
            if (aliases == null) aliases = new List<Definitions.Alias>();
            var currentAlias = property.stringValue;
            var aliasOptions = new string[aliases.Count + 1];
            aliasOptions[0] = "<None>";
            for (int i = 0; i < aliases.Count; i++)
            {
                aliasOptions[i + 1] = aliases[i].Key;
            }
            int currentIndex = System.Array.IndexOf(aliasOptions, currentAlias);
            var newIndex = EditorGUI.Popup(position, label.text, currentIndex, aliasOptions);
            if (newIndex != currentIndex)
            {
                property.stringValue = aliasOptions[newIndex];
                if (newIndex <= 0) property.stringValue = "";
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
