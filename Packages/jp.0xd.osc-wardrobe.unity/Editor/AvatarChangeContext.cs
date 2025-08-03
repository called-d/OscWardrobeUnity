using System;
using System.Collections.Generic;
using nadena.dev.ndmf;

namespace called_D.OscWardrobe.Unity.Editor
{

    public class AvatarChangeContext : IExtensionContext
    {
        public /* Definitions */ Dictionary<string, Dictionary<string, string>> definitions;

        public AvatarChangeContext()
        {
            definitions = new /* Definitions */ Dictionary<string, Dictionary<string, string>>();
            definitions.Add("*", new /* KeyValueToAlias */ Dictionary<string, string>());
        }

        public void OnActivate(BuildContext _context)
        {

        }

        public void OnDeactivate(BuildContext _context)
        {

        }

        public void AddDefinition(string condition, string parameter, string value, string alias)
        {
            if (String.IsNullOrEmpty(condition)) condition = "*";
            if (String.IsNullOrEmpty(parameter)) return;
            if (String.IsNullOrEmpty(value)) return;
            if (String.IsNullOrEmpty(alias)) return;
            AddDefinitionRaw(condition, $"/avatar/parameters/{parameter}={value}", alias);
        }

        public void AddDefinitionRaw(string condition1, string condition2, string alias)
        {
            if (!definitions.ContainsKey(condition1)) definitions[condition1] = new /* KeyValueToAlias */ Dictionary<string, string>();
            definitions[condition1][condition2] = alias;
        }
    }
}
