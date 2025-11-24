using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace called_D.OscWardrobe.Unity.Editor
{
    internal class Utils
    {

        [MenuItem("Tools/Osc Wardrobe/Open Definitions Directory")]
        static void OpenDefinitionsDirectory()
        {
            var path = Definitions.GetDefinitionRootDirectory();
            if (System.IO.Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer", path);
            }
        }

        public static string GetBlueprintId(GameObject obj)
        {
            if (!obj) return null;
            var pipelineManager = obj.GetComponent<VRC.Core.PipelineManager>();
            if (!pipelineManager) return null;
            var id = pipelineManager.blueprintId;
            if (string.IsNullOrEmpty(id)) return null;
            if (!id.StartsWith("avtr_")) return null; // Ensure it's a valid avatar blueprint ID
            return id;
        }

        public static VRCExpressionsMenu GetExpressionsMenu(GameObject obj)
        {
            if (!obj) return null;
            var avatarDescriptor = obj.GetComponent<VRCAvatarDescriptor>();
            if (!avatarDescriptor) return null;
            return avatarDescriptor.expressionsMenu;
        }

        private class Cache
        {
            private List<Definitions.Alias> _aliases;
            private DateTime _lastReadAliases;
            private float _aliasReadInterval = 300f;

            internal List<Definitions.Alias> GetAliases()
            {
                if (_aliases == null || (DateTime.Now - _lastReadAliases).TotalSeconds > _aliasReadInterval)
                {
                    _aliases = Definitions.ReadAvatarAliases();
                    _lastReadAliases = DateTime.Now;
                }
                return _aliases;
            }
            internal void SetAliases(List<Definitions.Alias> aliases)
            {
                _aliases = aliases;
                _lastReadAliases = DateTime.Now;
                Definitions.WriteAvatarAliases(aliases);
            }
        }
        private static Cache _instance;
        public static List<Definitions.Alias> Aliases
        {
            get
            {
                if (_instance == null) _instance = new Cache();
                return _instance.GetAliases();
            }
            set
            {
                if (_instance == null) _instance = new Cache();
                _instance.SetAliases(value);
            }
        }
    }
}
