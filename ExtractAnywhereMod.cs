using Colossal.IO.AssetDatabase;
using Colossal.Logging;
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

        internal ILog Logger { get; private set; } = default!;

        public static ExtractAnywhereOptions Options = default!;

        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            Logger.Info($"{nameof(OnCreateWorld)} started");

            Options = new ExtractAnywhereOptions(this);
            ExtractAnywhereOptionsManager.CreateInstance(Options);
            Options.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(nameof(ExtractAnywhereMod), Options, new ExtractAnywhereOptions(this));

            LoadLocales();

            Logger.Info($"{nameof(OnCreateWorld)} completed");
        }

        public void OnDispose()
        {
            Logger.Info($"{nameof(OnCreateWorld)} disposed");
        }

        public void OnLoad()
        {
            Logger = LogManager.GetLogger(nameof(ExtractAnywhereMod), showsErrorsInUI: false);
            Logger.Info($"{nameof(OnLoad)} completed");
        }
    }
}
