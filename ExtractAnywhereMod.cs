using Colossal.IO.AssetDatabase;
using ExtractAnywhere.Options;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace ExtractAnywhere
{
    public class ExtractAnywhereMod : IMod
    {
        private void LoadLocales()
        {
            foreach (string localeId in GameManager.instance.localizationManager.GetSupportedLocales())
            {
                GameManager.instance.localizationManager.AddSource(localeId, new LocaleEnGb(Options));
            }
        }

        public static ExtractAnywhereOptions Options = default!;

        public void OnLoad()
        {
        }

        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            Options = new ExtractAnywhereOptions(this);
            ExtractAnywhereOptionsManager.CreateInstance(Options);
            Options.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(nameof(ExtractAnywhereMod), Options, new ExtractAnywhereOptions(this));

            LoadLocales();
        }

        public void OnDispose()
        {
        }
    }
}
