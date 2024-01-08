using Colossal;
using Game.Areas;
using Game.Economy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtractAnywhere.Options
{
    public class LocaleEnGb : IDictionarySource
    {
        private readonly ExtractAnywhereOptions Options;

        public LocaleEnGb(ExtractAnywhereOptions options)
        {
            Options = options;
        }

        protected virtual string GetResourceNamePlural(Resource resource)
        {
            return resource switch
            {
                Resource.Coal => "coal",
                Resource.Cotton => "textile fibres",
                Resource.Grain => "grain",
                Resource.Oil => "oil",
                Resource.Ore => "ore",
                Resource.Vegetables => "vegetables",
                _ => "unknown"
            };
        }

        protected virtual string GetResourceDepositName(Resource resource)
        {
            return resource switch
            {
                Resource.Coal => "an ore deposit",
                Resource.Cotton => "fertile land",
                Resource.Grain => "fertile land",
                Resource.Oil => "an oil deposit",
                Resource.Ore => "an ore deposit",
                Resource.Vegetables => "fertile land",
                _ => "Unknown"
            };
        }

        protected virtual string GetMapFeatureName(MapFeature feature)
        {
            return feature switch
            {
                MapFeature.FertileLand => "Fertile Land",
                MapFeature.Forest => "Forest",
                MapFeature.Oil => "Oil",
                MapFeature.Ore => "Ore",
                _ => "Unknown"
            };
        }

        protected virtual string GetExtractorName(Resource resource)
        {
            return resource switch
            {
                Resource.Coal => "coal",
                Resource.Cotton => "textile fibres",
                Resource.Grain => "grain",
                Resource.Livestock => "livestock",
                Resource.Oil => "oil",
                Resource.Ore => "ore",
                Resource.Stone => "stone",
                Resource.Vegetables => "vegetables",
                Resource.Wood => "forestry",
                _ => "unknown"
            };
        }

        protected virtual void AddLocalizationEntry(Dictionary<string, string> entries, Resource resource)
        {
            string canExtractAnywhereLocaleId = $"{resource}.{nameof(Options.CanExtractAnywhere)}";
            string resourceName = GetResourceNamePlural(resource);

            entries.Add(
                Options.GetOptionLabelLocaleID(canExtractAnywhereLocaleId),
                string.Join(' ', resourceName.Split(' ').Select(s => char.ToUpperInvariant(s[0]) + s[1..]))
            );

            string resourceDepositName = GetResourceDepositName(resource);

            entries.Add(
                Options.GetOptionDescLocaleID(canExtractAnywhereLocaleId),
                $"Toggle whether or not {resourceName} can be extracted without requiring {resourceDepositName}. Turning this on means that extraction does not require {resourceDepositName}."
            );
        }

        protected virtual void AddLocalizationEntry(Dictionary<string, string> entries, MapFeature feature)
        {
            string featureName = Enum.GetName(typeof(MapFeature), feature);
            string maxExtractionRadiusLocaleId = $"{feature}.{nameof(Options.ExtractionRadius)}";
            string friendlyFeatureName = GetMapFeatureName(feature);

            // Tab.

            entries.Add(
                Options.GetOptionTabLocaleID(featureName),
                friendlyFeatureName
            );

            // MaxExtractionRadius.

            entries.Add(
                Options.GetOptionLabelLocaleID(maxExtractionRadiusLocaleId),
                "Extraction radius multiplier"
            );

            entries.Add(
                Options.GetOptionDescLocaleID(maxExtractionRadiusLocaleId),
                $"Set the multiplier for the area size of the extractors. The default is 1 (base area size is {ExtractorRadiusOption.DefaultExtractionRadii[feature]}). This will affect the following resource extractors: {string.Join(", ", Options.ExtractionRadius.GetMapFeatureSupportedResources(feature).Select(GetExtractorName))}."
            );

            entries.Add(Options.GetOptionLabelLocaleID($"{feature}.Reset"), "Reset to default");
            entries.Add(Options.GetOptionWarningLocaleID($"{feature}.Reset"), $"This will reset all {friendlyFeatureName} settings to their default values. Are you sure you want to continue?");
        }

        public virtual IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            var entries = new Dictionary<string, string>
            {
                [Options.GetSettingsLocaleID()] = "Extract Anywhere",

                [Options.GetOptionLabelLocaleID(nameof(ExtractAnywhereOptionsManager.CanAnyExtractAnywhere))] = "Can all extractors extract anywhere?",
                [Options.GetOptionDescLocaleID(nameof(ExtractAnywhereOptionsManager.CanAnyExtractAnywhere))] = "Toggle all extractors' abilities to extract resources without the need for a natural resource deposit.",

                [Options.GetOptionLabelLocaleID(nameof(ExtractAnywhereOptionsManager.MaxExtractionRadius))] = "Extraction radius multiplier",
                [Options.GetOptionDescLocaleID(nameof(ExtractAnywhereOptionsManager.MaxExtractionRadius))] = "Set the multiplier for the area sizes of all extractors.",

                [Options.GetOptionTabLocaleID("General")] = "General",
                [Options.GetOptionGroupLocaleID(nameof(Options.CanExtractAnywhere))] = "Extract Anywhere",

                [Options.GetOptionLabelLocaleID("Reset")] = "Reset to default",
                [Options.GetOptionWarningLocaleID("Reset")] = "This will reset all Extract Anywhere mod settings to their default values. Are you sure you want to continue?"
            };

            foreach (Resource resource in Options.CanExtractAnywhere.SupportedResources)
            {
                AddLocalizationEntry(entries, resource);
            }

            foreach (MapFeature feature in Options.ExtractionRadius.SupportedFeatures)
            {
                AddLocalizationEntry(entries, feature);
            }

            return entries;
        }

        public virtual void Unload()
        {
        }
    }
}