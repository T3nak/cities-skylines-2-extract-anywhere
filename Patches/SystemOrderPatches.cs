using ExtractAnywhere.Systems;
using Game;
using Game.Common;
using HarmonyLib;

namespace ExtractAnywhere.Patches
{
    [HarmonyPatch]
    public class SystemOrderPatches
    {
        [HarmonyPatch(typeof(SystemOrder), nameof(SystemOrder.Initialize), new[] { typeof(UpdateSystem) })]
        [HarmonyPostfix]
        private static void InjectSystems(UpdateSystem updateSystem)
        {
            updateSystem.UpdateAt<ExtractAnywhereResourceSystem>(SystemUpdatePhase.PrefabUpdate);
            Plugin.Mod.OnCreateWorld(updateSystem);
        }
    }
}
