using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace called_D.OscWardrobe.Unity.Editor
{
    public class Definitions
    {
        private static string identifier = "jp.0xd.osc-wardrobe.app";

        private static string CreateDirectoryIfNotExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetDefinitionRootDirectory()
        {
            var appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            var path = System.IO.Path.Combine(appData, identifier, "defs");
            return CreateDirectoryIfNotExists(path);
        }
        private static string AvatarsDirectory() =>
            CreateDirectoryIfNotExists(System.IO.Path.Combine(GetDefinitionRootDirectory(), "avatars"));

        public static void WriteAvatarDefinition(string blueprintId, string avatar)
        {
            var path = System.IO.Path.Combine(AvatarsDirectory(), $"{blueprintId}.json");
            System.IO.File.WriteAllText(path, avatar);
        }

        public static string ReadAvatarDefinition(string blueprintId)
        {
            var path = System.IO.Path.Combine(AvatarsDirectory(), $"{blueprintId}.json");
            if (System.IO.File.Exists(path)) return System.IO.File.ReadAllText(path);
            return null;
        }

        public class Alias
        {
            public string Key;
            public string BlueprintId;
            public string AvatarName;

            public override string ToString()
            {
                if (AvatarName != null && AvatarName != "")
                    return $"[Alias] {Key}={BlueprintId} ({AvatarName})";
                return $"[Alias] {Key}={BlueprintId}";
            }
        }

        public static List<Alias> ReadAvatarAliases()
        {
            var path = System.IO.Path.Combine(GetDefinitionRootDirectory(), "aliases.json");
            if (!System.IO.File.Exists(path)) return new List<Alias>();
            var content = System.IO.File.ReadAllText(path);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            if (dict == null) return new List<Alias>();
            return dict.Select(kv => new Alias { Key = kv.Key, BlueprintId = kv.Value }).ToList();
        }
        public static void WriteAvatarAliases(List<Alias> aliases)
        {
            var path = System.IO.Path.Combine(GetDefinitionRootDirectory(), "aliases.json");
            var dict = aliases.ToDictionary(a => a.Key, a => a.BlueprintId);
            var content = JsonConvert.SerializeObject(dict, Formatting.Indented);
            System.IO.File.WriteAllText(path, content);
        }
    }
}
