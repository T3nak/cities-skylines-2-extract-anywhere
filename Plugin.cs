using BepInEx;
using BepInEx.Logging;
using ExtractAnywhere.Extensions;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if BEPINEX_V6
    using BepInEx.Unity.Mono;
#endif

namespace ExtractAnywhere
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal static ExtractAnywhereMod Mod;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfoBanner($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded.");

            Mod = new();
            Mod.OnLoad();

            var harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MyPluginInfo.PLUGIN_GUID + "_Cities2Harmony");
            var patchedMethods = harmony.GetPatchedMethods().ToArray();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} made patches. Patched methods: " + patchedMethods.Length);

            foreach (var patchedMethod in patchedMethods)
            {
                Logger.LogInfo($"Patched method: {patchedMethod.Module.Name}:{patchedMethod.Name}");
            }
        }

        // Keep in mind, Unity UI is immediate mode, so OnGUI is called multiple times per frame
        // https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnGUI.html
        private void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
