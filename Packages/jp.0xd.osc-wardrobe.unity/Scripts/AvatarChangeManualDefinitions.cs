using System;
using UnityEngine;
using UnityEngine.Animations;

namespace called_D.OscWardrobe.Unity
{
    public class AvatarChangeManualDefinitions : MonoBehaviour, VRC.SDKBase.IEditorOnly
    {
        [NotKeyable][SerializeField] public string Condition = "*";
        [NotKeyable][SerializeField] public ParameterDefinition[] Definitions;

        [Serializable] public class ParameterDefinition
        {
            [NotKeyable][SerializeField] public string Parameter;
            [NotKeyable][SerializeField] public string Value;
            [NotKeyable][SerializeField][AvatarAlias] public string Alias;
        }
    }
}
