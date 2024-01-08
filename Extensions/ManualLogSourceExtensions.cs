using BepInEx.Logging;

namespace ExtractAnywhere.Extensions
{
    internal static class ManualLogSourceExtensions
    {
        private const string BannerLeading = "> > > > >  ";
        private const string BannerTrailing = "  < < < < <";

        public static void LogInfoBanner(this ManualLogSource logger, string message)
        {
            string bannerPadding = new string(' ', message.Length);
            string bannerHeading = $"{BannerLeading}{bannerPadding}{BannerTrailing}";

            logger.LogInfo(bannerHeading);
            logger.LogInfo($"{BannerLeading}{message}{BannerTrailing}");
            logger.LogInfo(bannerHeading);
        }
    }
}
