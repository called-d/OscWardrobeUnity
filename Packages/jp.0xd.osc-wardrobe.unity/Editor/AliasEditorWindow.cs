using UnityEngine;
using UnityEditor;

namespace called_D.OscWardrobe.Unity.Editor
{
    public class AliasEditorWindow : EditorWindow
    {

        [MenuItem("Window/Osc Wardrobe/Alias Editor")]
        public static void ShowWWindow()
        {
            AliasEditorWindow window = GetWindow<AliasEditorWindow>("Alias Editor");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Add Alias Definition", EditorStyles.boldLabel);

            // condition = EditorGUILayout.TextField("Condition", condition);
            // parameter = EditorGUILayout.TextField("Parameter", parameter);
            // value = EditorGUILayout.TextField("Value", value);
            // alias = EditorGUILayout.TextField("Alias", alias);

            // if (GUILayout.Button("Add Definition"))
            // {
            //     AddDefinition(condition, parameter, value, alias);
            //     ClearFields();
            // }
        }

        // private void AddDefinition(string condition, string parameter, string value, string alias)
        // {
        //     // Here you would add the logic to add the definition to your context
        //     Debug.Log($"Added Definition: Condition={condition}, Parameter={parameter}, Value={value}, Alias={alias}");
        // }

        // private void ClearFields()
        // {
        //     condition = "*";
        //     parameter = "";
        //     value = "";
        //     alias = "";
        // }
    }
}
