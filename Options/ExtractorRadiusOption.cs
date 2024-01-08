using Game.Areas;
using Game.Economy;
using Game.Settings;
using System.Collections.Generic;

namespace ExtractAnywhere.Options
{
    public class ExtractorRadiusOption : ExtractorOption<Dictionary<MapFeature, float>>
    {
        public static Dictionary<MapFeature, float> DefaultExtractionRadii = new()
        {
            [MapFeature.FertileLand] = 300,
            [MapFeature.Forest] = 400,
            [MapFeature.Oil] = 300,
            [MapFeature.Ore] = 400,
        };

        [SettingsUIHidden]
        public readonly HashSet<MapFeature> SupportedFeatures = new()
        {
            MapFeature.FertileLand,
            MapFeature.Forest,
            MapFeature.Oil,
            MapFeature.Ore
        };

        [SettingsUIHidden]
        public readonly HashSet<Resource> SupportedResources = new()
        {
            Resource.Coal,
            Resource.Cotton,
            Resource.Grain,
            Resource.Oil,
            Resource.Ore,
            Resource.Stone,
            Resource.Vegetables,
            Resource.Wood
        };

        public override void SetDefaults()
        {
            base.SetDefaults();

            foreach (MapFeature feature in SupportedFeatures)
            {
                Value[feature] = 1;
            }

            OnValueChanged();
        }

        public virtual void SetDefaults(MapFeature feature)
        {
            Value[feature] = 1;

            OnValueChanged();
        }

        public virtual float GetResourceExtractionRadius(MapFeature feature)
        {
            float extractionRadius = Value.TryGetValue(feature, out float radius)
                ? radius
                : 1;

            return extractionRadius;
        }

        public virtual void SetResourceExtractionRadius(MapFeature feature, float radius)
        {
            Value[feature] = radius;
            OnValueChanged();
        }

        public virtual bool IsMapFeatureSupported(MapFeature feature) => SupportedFeatures.Contains(feature);

        public virtual bool IsResourceSupported(Resource resource) => SupportedResources.Contains(resource);

        public virtual MapFeature GetResourceSupportedMapFeature(Resource resource)
        {
            return resource switch
            {
                Resource.Cotton
                    or Resource.Grain
                    or Resource.Vegetables
                    => MapFeature.FertileLand,
                Resource.Wood => MapFeature.Forest,
                Resource.Oil => MapFeature.Oil,
                Resource.Coal
                    or Resource.Ore
                    or Resource.Stone
                    => MapFeature.Ore,
                _ => MapFeature.None,
            };
        }

        public virtual HashSet<Resource> GetMapFeatureSupportedResources(MapFeature feature)
        {
            return feature switch
            {
                MapFeature.FertileLand => new HashSet<Resource>
                {
                    Resource.Cotton,
                    Resource.Grain,
                    Resource.Livestock,
                    Resource.Vegetables
                },
                MapFeature.Forest => new HashSet<Resource>
                {
                    Resource.Wood
                },
                MapFeature.Oil => new HashSet<Resource>
                {
                    Resource.Oil
                },
                MapFeature.Ore => new HashSet<Resource>
                {
                    Resource.Coal,
                    Resource.Ore,
                    Resource.Stone
                },
                _ => new HashSet<Resource>(),
            };
        }
    }
}
