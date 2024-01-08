using static Game.UI.Menu.AutomaticSettings;

namespace ExtractAnywhere.Extensions
{
    public static class SettingTabDataExtensions
    {
        public static SettingTabData FluentAddItem(this SettingTabData settingTabData, SettingItemData item)
        {
            settingTabData.AddItem(item);
            return settingTabData;
        }
    }
}
