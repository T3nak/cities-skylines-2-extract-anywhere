using Game.Areas;
using Game.Economy;
using Game.Settings;
using System.Collections.Generic;
using System.Linq;

namespace ExtractAnywhere.Options
{
    public class CanExtractAnywhereOption : ExtractorOption<Dictionary<Resource, bool>>
    {
        private readonly Dictionary<MapFeature, HashSet<Resource>> _mapFeatureSupportedResources = new()
        {
            [MapFeature.FertileLand] = new()
            {
                Resource.Cotton,
                Resource.Grain,
                Resource.Vegetables
            },
            [MapFeature.Oil] = new()
            {
                Resource.Oil
            },
            [MapFeature.Ore] = new()
            {
                Resource.Coal,
                Resource.Ore
            }
        };

        [SettingsUIDeveloper]
        public Dictionary<MapFeature, HashSet<Resource>> MapFeatureSupportedResources => _mapFeatureSupportedResources;

        [SettingsUIDeveloper]
        public HashSet<Resource> SupportedResources => MapFeatureSupportedResources.Values
            .SelectMany(r => r)
            .ToHashSet();

        public override void SetDefaults()
        {
            base.SetDefaults();

            foreach (Resource resource in SupportedResources)
            {
                Value[resource] = true;
            }

            OnValueChanged();
        }

        public virtual void SetDefaults(MapFeature feature)
        {
            if (MapFeatureSupportedResources.TryGetValue(feature, out HashSet<Resource> resources))
            {
                foreach (Resource resource in resources)
                {
                    Value[resource] = true;
                }
            }

            OnValueChanged();
        }

        public virtual bool IsResourceSupported(Resource resource) => SupportedResources.Contains(resource);

        public virtual bool IsResourceSupported(ResourceInEditor resource) => IsResourceSupported(EconomyUtils.GetResource(resource));

        public virtual bool CanResourceBeExtractedAnywhere(Resource resource) => Value.TryGetValue(resource, out bool canResourceBeExtractedAnywhere) && canResourceBeExtractedAnywhere;

        public virtual bool CanResourceBeExtractedAnywhere(ResourceInEditor resource) => CanResourceBeExtractedAnywhere(EconomyUtils.GetResource(resource));

        public virtual bool ToggleResourceExtractionAnywhere(Resource resource, bool? isEnabled = default)
        {
            bool isAlreadyEnabled = CanResourceBeExtractedAnywhere(resource);
            bool effectiveIsEnabled = isEnabled ?? !isAlreadyEnabled;

            Value[resource] = effectiveIsEnabled;

            OnValueChanged();

            return effectiveIsEnabled;
        }

        public virtual bool IsMapFeatureSupported(MapFeature feature)
        {
            return MapFeatureSupportedResources.ContainsKey(feature);
        }
    }
}
