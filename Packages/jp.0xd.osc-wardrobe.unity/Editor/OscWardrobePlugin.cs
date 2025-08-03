using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nadena.dev.ndmf;
using VRC.SDK3.Avatars.ScriptableObjects;
using nadena.dev.modular_avatar.core;
using Newtonsoft.Json;


[assembly: ExportsPlugin(typeof(called_D.OscWardrobe.Unity.Editor.OscWardrobePlugin))]
namespace called_D.OscWardrobe.Unity.Editor
{
    public class OscWardrobePlugin : Plugin<OscWardrobePlugin>
    {
        public override string QualifiedName => "jp.0xd.osc-wardrobe.unity";
        public override string DisplayName => "OSC Wardrobe";

        protected override void Configure()
        {
            var Generating = InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar");
            Generating.WithRequiredExtension(typeof(AvatarChangeContext), seq =>
            {
                seq.Run("Convert Menu Definitions", ctx =>
                {
                    Debug.Log("Converting menu definitions for OSC Wardrobe...");

                    var newParameters = new Dictionary<string, VRCExpressionParameters.Parameter>();

                    var avatarChangeContext = ctx.Extension<AvatarChangeContext>();
                    var avatarRoot = ctx.AvatarRootObject;
                    var manualDefinitions = avatarRoot.GetComponentsInChildren<AvatarChangeManualDefinitions>(true);
                    if (manualDefinitions != null)
                        foreach (var def in manualDefinitions)
                            foreach (var param in def.Definitions)
                                avatarChangeContext.AddDefinition(def.Condition, param.Parameter, param.Value, param.Alias);
                    var menuItems = avatarRoot.GetComponentsInChildren<AvatarChangeMenuItem>(true);
                    if (menuItems != null && menuItems.Length > 0)
                    {
                        var obj = new GameObject("AvatarChangeParameters");
                        obj.transform.SetParent(avatarRoot.transform, false);
                        var maParameters = obj.AddComponent<ModularAvatarParameters>();
                        foreach (var item in menuItems)
                        {
                            if (item.TryConvertToModularAvatarMenuItem(out var _maMenuItem))
                            {
                                // TODO: get condition from parent of menu item
                                avatarChangeContext.AddDefinition("*", item.Parameter, item.Value, item.Alias);
                                maParameters.parameters.Add(new ParameterConfig
                                {
                                    nameOrPrefix = item.Parameter,
                                    internalParameter = false,
                                    isPrefix = false,
                                    syncType = ParameterSyncType.Int,
                                    localOnly = true,
                                    saved = false,
                                });
                            }
                        }
                    }

                });
            });

            var Optimizing = InPhase(BuildPhase.Optimizing);
            Optimizing.WithRequiredExtension(typeof(AvatarChangeContext), seq => {
                seq.Run("Write Definitions", ctx =>
                {
                    Debug.Log("Writing definitions for OSC Wardrobe...");
                    var avatarRoot = ctx.AvatarRootObject;
                    var blueprintId = Utils.GetBlueprintId(avatarRoot);
                    if (blueprintId == null)
                    {
                        Debug.LogWarning("OSC Wardrobe: No valid blueprint ID found on avatar root object.");
                        return;
                    }
                    var definitions = ctx.Extension<AvatarChangeContext>().definitions;
                    Definitions.WriteAvatarDefinition(blueprintId, JsonConvert.SerializeObject(definitions, Formatting.Indented));
                });
            });
        }
    }
}
