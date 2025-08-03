using System;
using UnityEngine;
using UnityEngine.Animations;
using nadena.dev.modular_avatar.core;

namespace called_D.OscWardrobe.Unity
{
    public class AvatarChangeMenuItem : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        [NotKeyable][SerializeField] public string MenuName;
        [NotKeyable][SerializeField] public string Parameter;
        [NotKeyable][SerializeField] public string Value;
        [NotKeyable][SerializeField][AvatarAlias] public string Alias;

        public bool TryConvertToModularAvatarMenuItem(out ModularAvatarMenuItem menuItem)
        {
            menuItem = null;
            if (string.IsNullOrEmpty(MenuName) || string.IsNullOrEmpty(Parameter) || string.IsNullOrEmpty(Value) || string.IsNullOrEmpty(Alias)) return false;
            if (!float.TryParse(Value, out var v)) return false;

            var existingItem = gameObject.GetComponent<ModularAvatarMenuItem>();
            menuItem = existingItem == null ? gameObject.AddComponent<ModularAvatarMenuItem>() : existingItem;

            menuItem.Control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Button;
            menuItem.isSynced = false;
            menuItem.isSaved = false;
            menuItem.label = MenuName;
            menuItem.Control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter
            {
                name = Parameter,
            };
            menuItem.Control.value = v;

            return menuItem != null;
        }
    }
}
