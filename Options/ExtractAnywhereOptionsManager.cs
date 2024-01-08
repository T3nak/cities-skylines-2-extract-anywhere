using Game.Areas;
using Game.Economy;
using System.Linq;

namespace ExtractAnywhere.Options
{
    public class ExtractAnywhereOptionsManager
    {
        public readonly ExtractAnywhereOptions Options;

        public ExtractAnywhereOptionsManager(ExtractAnywhereOptions options)
        {
            Options = options;
        }

        public static ExtractAnywhereOptionsManager? Instance;

        public virtual bool CanAnyExtractAnywhere
        {
            get => Options.CanExtractAnywhere.Value.Any(r => r.Value);
            set
            {
                if (CanAnyExtractAnywhere)
                {
                    foreach (Resource resource in Options.CanExtractAnywhere.SupportedResources)
                    {
                        Options.CanExtractAnywhere.ToggleResourceExtractionAnywhere(resource, false);
                    }
                }
                else
                {
                    foreach (Resource resource in Options.CanExtractAnywhere.SupportedResources)
                    {
                        Options.CanExtractAnywhere.ToggleResourceExtractionAnywhere(resource, true);
                    }
                }
            }
        }

        public virtual float MaxExtractionRadius
        {
            get => Options.ExtractionRadius.Value.Any()
                ? Options.ExtractionRadius.Value.Max(kvp => kvp.Value)
                : 1;
            set
            {
                foreach (MapFeature feature in Options.ExtractionRadius.SupportedFeatures)
                {
                    Options.ExtractionRadius.SetResourceExtractionRadius(feature, value);
                }
            }
        }

        public static ExtractAnywhereOptionsManager CreateInstance(ExtractAnywhereOptions options)
        {
            Instance = new ExtractAnywhereOptionsManager(options);
            return Instance;
        }
    }
}
