using Colossal.Serialization.Entities;
using ExtractAnywhere;
using Game;
using Game.SceneFlow;
using HarmonyLib;

namespace ExtendedRoadUpgrades.Patches
{
    // Thanks to https://github.com/ST-Apps/CS2-ExtendedRoadUpgrades.
    [HarmonyPatch]
    public class GameManagerPatches
    {
        private static GameManager? GameManager;

        private static void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            if (mode == GameMode.MainMenu)
            {
                ExtractAnywhereMod.Options.Version = 1;
                ExtractAnywhereMod.Options.ApplyAndSave();

                GameManager!.onGameLoadingComplete -= OnGameLoadingComplete;
                GameManager = default;
            }
            else
            {
                GameManager!.onGameLoadingComplete -= OnGameLoadingComplete;
                GameManager = default;
            }
        }

        [HarmonyPatch(typeof(GameManager), nameof(InitializeThumbnails))]
        [HarmonyPrefix]
        private static void InitializeThumbnails(GameManager __instance)
        {
            GameManager = __instance;
            GameManager.onGameLoadingComplete += OnGameLoadingComplete;
        }
    }
}
