using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace called_D.OscWardrobe.Unity.Editor
{
    public class AliasEditorWindow : EditorWindow
    {
        private static AliasEditorWindow _window;
        private int _editingIndex = -1;
        private string aliasKey = "";
        private string blueprintId = "";
        private Vector2 _scroll1;
        private Vector2 _scroll2;
        private bool _isDirty = false;

        [MenuItem("Window/Osc Wardrobe/Alias Editor")]
        public static void ShowWWindow()
        {
            AliasEditorWindow window = GetWindow<AliasEditorWindow>("Alias Editor");
            _window = window;
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Add Alias Definition", EditorStyles.boldLabel);

            var aliases = Utils.Aliases;
            var aliasMap = aliases.GroupBy(a => a.BlueprintId).ToDictionary(g => g.Key, _ => true);
            _scroll1 = GUILayout.BeginScrollView(_scroll1, GUILayout.Height(Mathf.Min(300, aliases.Count * 20 + 10)));
            var style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft;
            var oldEditingIndex = _editingIndex;
            _editingIndex = GUILayout.SelectionGrid(
                _editingIndex,
                aliases.Select((a, i) => $"{(i == _editingIndex ? "✏ " : "  ")}{a.Key} ({a.BlueprintId})").ToArray(),
                1,
                style
            );
            GUILayout.EndScrollView();

            if (oldEditingIndex != _editingIndex)
            {
                GUI.FocusControl(null);
                if (_editingIndex >= 0 && _editingIndex < aliases.Count)
                {
                    var editingAlias = aliases[_editingIndex];
                    aliasKey = editingAlias.Key;
                    blueprintId = editingAlias.BlueprintId;
                }
                else
                {
                    ClearFields();
                }
            }

            aliasKey = EditorGUILayout.TextField("Alias Key", aliasKey);
            blueprintId = EditorGUILayout.TextField("Blueprint ID", blueprintId);

            var changed = _editingIndex < 0
                ? !(aliasKey == "" && blueprintId == "")
                : !(
                   aliasKey == aliases[_editingIndex].Key &&
                   blueprintId == aliases[_editingIndex].BlueprintId
                );

            GUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(_editingIndex < 0 ? !changed : false)) {
                if (GUILayout.Button("↰"))
                {
                    ClearFields();
                    _editingIndex = -1;
                }
            }
            var originalBackgroundColor = GUI.backgroundColor;
            using (new EditorGUI.DisabledScope(_editingIndex < 0))
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("－"))
                {
                    aliases.RemoveAt(_editingIndex);
                    Utils.Aliases = aliases;
                    ClearFields();
                    _editingIndex = -1;
                }
                GUI.backgroundColor = originalBackgroundColor;
            }
            var canCreate = aliasKey != "" && blueprintId != "";
            using (new EditorGUI.DisabledScope(!(canCreate && changed)))
            {
                GUI.backgroundColor = Color.green;
                if (_editingIndex < 0)
                {
                    if (GUILayout.Button("＋"))
                    {
                        CreateNewDefinition();
                    }
                } else
                {
                    if (GUILayout.Button("✓"))
                    {
                        var editingAlias = aliases[_editingIndex];
                        editingAlias.Key = aliasKey;
                        editingAlias.BlueprintId = blueprintId;
                        Utils.Aliases = aliases;
                        ClearFields();
                        _editingIndex = -1;
                    }
                }
                GUI.backgroundColor = originalBackgroundColor;
            }
            GUILayout.EndHorizontal();

            _scroll2 = GUILayout.BeginScrollView(_scroll2);
            foreach (var paramFile in ParamFiles)
            {
                if (aliasMap.ContainsKey(paramFile.id)) continue;
                GUILayout.BeginHorizontal();
                var isEditing = blueprintId == paramFile.id;
                using (new EditorGUI.DisabledScope(isEditing)) {
                    if (GUILayout.Button(isEditing ? "✏" : "⇧", GUILayout.Width(22)))
                    {
                        aliasKey = paramFile.name;
                        blueprintId = paramFile.id;
                        GUI.FocusControl(null);
                        _editingIndex = -1;
                    }
                }
                GUILayout.Label($"{paramFile.name} ({paramFile.id})", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("❏", GUILayout.Width(22)))
                {
                    EditorGUIUtility.systemCopyBuffer = paramFile.id;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        private void Update()
        {
            if (_isDirty)
            {
                _instance.SortParamFiles();
                Repaint();
                _isDirty = false;
                Debug.Log($"count: {ParamFiles.Count}");
            }
        }
        private void CreateNewDefinition()
        {
            if (aliasKey == "" || blueprintId == "") return;
            var aliases = Utils.Aliases;
            aliases.Add(new Definitions.Alias
            {
                Key = aliasKey,
                BlueprintId = blueprintId,
            });
            Utils.Aliases = aliases;
            ClearFields();
        }

        private void ClearFields()
        {
            aliasKey = "";
            blueprintId = "";
        }

        public static string GetAvatarParameterFilesRootDirectory()
        {
            var appData = System.IO.Directory.GetParent(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)).FullName;
            var path = System.IO.Path.Combine(appData, "LocalLow", "VRChat", "VRChat", "OSC");
            return path;
        }

        public class AvatarParameter
        {
            public string id;
            public string name;
            public System.DateTime? lastModified;
        }

        private class Cache
        {
            private List<AvatarParameter> _paramFiles;
            internal List<AvatarParameter> GetParamFiles()
            {
                if (_paramFiles == null)
                {
                    _paramFiles = ReadAllParamFiles();
                }
                return _paramFiles;
            }
            internal void SortParamFiles()
            {
                if (_paramFiles == null) return;
                _paramFiles.Sort((a, b) => System.DateTime.Compare(
                    b.lastModified ?? System.DateTime.MinValue,
                    a.lastModified ?? System.DateTime.MinValue
                ));
            }

            private List<AvatarParameter> ReadAllParamFiles()
            {
                var context = System.Threading.SynchronizationContext.Current;
                var paramFiles = new List<AvatarParameter>();
                var paths = System.IO.Directory.GetFiles(GetAvatarParameterFilesRootDirectory(), "*.json", System.IO.SearchOption.AllDirectories);

                Task.Run(async () => {
                    await Task.WhenAll(paths.Select(async (path) => {
                        var param = await ReadParamFile(path);
                        if (param == null) return;
                        context.Post(param => {
                            paramFiles.Add(param as AvatarParameter);
                        }, param);
                        _window._isDirty = true;
                    }).ToList());
                });

                return paramFiles;
            }
            private async Task<AvatarParameter> ReadParamFile(string path)
            {
                var content = await System.IO.File.ReadAllTextAsync(path);
                Debug.Log($"{path} {content}");
                var param = JsonConvert.DeserializeObject<AvatarParameter>(content);
                if (param != null) param.lastModified = System.IO.File.GetLastWriteTime(path);
                return param;
            }
        }
        private static Cache _instance;
        private static List<AvatarParameter> ParamFiles
        {
            get
            {
                if (_instance == null) _instance = new Cache();
                return _instance.GetParamFiles();
            }
        }
    }
}
