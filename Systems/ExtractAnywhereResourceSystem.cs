using Colossal.Entities;
using ExtractAnywhere.Options;
using Game;
using Game.Areas;
using Game.Economy;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace ExtractAnywhere.Systems
{
    public class ExtractAnywhereResourceSystem : GameSystemBase
    {
        protected PrefabSystem? PrefabSystem;

        protected EntityQuery? ResourcePrefabQuery;

        protected EntityQuery? BuildingPropertyDataQuery;

        protected int PreviousExtractAnywhereOptionsVersion;

        protected override void OnCreate()
        {
            base.OnCreate();

            PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            ResourcePrefabQuery = GetEntityQuery(
                ComponentType.ReadWrite<PrefabData>(),
                ComponentType.ReadWrite<ResourceData>()
            );

            BuildingPropertyDataQuery = GetEntityQuery(
                ComponentType.ReadOnly<BuildingPropertyData>(),
                ComponentType.ReadOnly<PlaceholderBuildingData>()
            );
        }

        protected override void OnUpdate()
        {
            UpdateResourcePrefabData();
            UpdateLotPrefabData();
            PreviousExtractAnywhereOptionsVersion = ExtractAnywhereMod.Options.Version;
        }

        protected virtual void UpdateResourcePrefabData()
        {
            if (
                ResourcePrefabQuery.HasValue
                && !ResourcePrefabQuery.Value.IsEmptyIgnoreFilter
                && ExtractAnywhereMod.Options.Version != PreviousExtractAnywhereOptionsVersion
            )
            {
                foreach (Entity resourceEntity in ResourcePrefabQuery.Value.ToEntityArray(Allocator.Temp))
                {
                    ResourcePrefab resourcePrefab = PrefabSystem!.GetPrefab<ResourcePrefab>(resourceEntity);
                    ResourceInEditor resource = resourcePrefab.m_Resource;

                    if (!ExtractAnywhereMod.Options.CanExtractAnywhere.IsResourceSupported(resource))
                    {
                        continue;
                    }

                    if (!EntityManager.TryGetComponent(resourceEntity, out ResourceData resourceData))
                    {
                        continue;
                    }

                    bool canResourceBeExtractedAnywhere = ExtractAnywhereMod.Options.CanExtractAnywhere.CanResourceBeExtractedAnywhere(resource);

                    if (canResourceBeExtractedAnywhere == !resourceData.m_RequireNaturalResource)
                    {
                        continue;
                    }

                    Plugin.Logger.LogInfo($"[{nameof(ExtractAnywhereResourceSystem)}.{nameof(UpdateResourcePrefabData)}] {resource} {!resourceData.m_RequireNaturalResource} → {canResourceBeExtractedAnywhere}");

                    resourcePrefab.m_RequireNaturalResource = !canResourceBeExtractedAnywhere;
                    resourceData.m_RequireNaturalResource = resourcePrefab.m_RequireNaturalResource;

                    EntityManager.SetComponentData(resourceEntity, resourceData);
                }
            }
        }

        protected virtual void UpdateLotPrefabData()
        {
            if (
                !BuildingPropertyDataQuery.HasValue
                || BuildingPropertyDataQuery.Value.IsEmptyIgnoreFilter
                || ExtractAnywhereMod.Options.Version == PreviousExtractAnywhereOptionsVersion
            )
            {
                return;
            }

            foreach (Entity entity in BuildingPropertyDataQuery.Value.ToEntityArray(Allocator.Temp))
            {
                if (
                    !EntityManager.TryGetComponent(entity, out BuildingPropertyData buildingPropertyData)
                    || !ExtractAnywhereMod.Options.ExtractionRadius.IsResourceSupported(buildingPropertyData.m_AllowedManufactured)
                )
                {
                    continue;
                }

                Resource resource = buildingPropertyData.m_AllowedManufactured;

                if (
                    !EntityManager.TryGetComponent(entity, out PlaceholderBuildingData placeholderBuildingData)
                    || placeholderBuildingData.m_Type != BuildingType.ExtractorBuilding
                )
                {
                    continue;
                }

                if (!EntityManager.TryGetBuffer(entity, isReadOnly: false, out DynamicBuffer<Game.Prefabs.SubArea> subAreaBuffer))
                {
                    continue;
                }

                foreach (Game.Prefabs.SubArea subArea in subAreaBuffer.ToNativeArray(Allocator.Temp))
                {
                    Entity subAreaEntity = subArea.m_Prefab;

                    if (!EntityManager.TryGetComponent(subAreaEntity, out ExtractorAreaData _))
                    {
                        continue;
                    }

                    if (!EntityManager.TryGetComponent(subAreaEntity, out LotData lotData))
                    {
                        continue;
                    }

                    MapFeature feature = ExtractAnywhereMod.Options.ExtractionRadius.GetResourceSupportedMapFeature(resource);

                    float lotDataRadius = lotData.m_MaxRadius;
                    ExtractorRadiusOption.DefaultExtractionRadii.TryAdd(feature, lotDataRadius);
                    float defaultRadius = ExtractorRadiusOption.DefaultExtractionRadii[feature];
                    float extractorRadius = ExtractAnywhereMod.Options.ExtractionRadius.GetResourceExtractionRadius(feature) * defaultRadius;

                    if (lotDataRadius == extractorRadius)
                    {
                        continue;
                    }

                    Plugin.Logger.LogInfo($"[{nameof(ExtractAnywhereResourceSystem)}.{nameof(UpdateLotPrefabData)}] {feature} {lotDataRadius} → {extractorRadius}");

                    lotData.m_MaxRadius = extractorRadius;

                    EntityManager.SetComponentData(subAreaEntity, lotData);
                }
            }
        }
    }
}
