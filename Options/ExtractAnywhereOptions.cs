using Colossal.IO.AssetDatabase;
using ExtractAnywhere.Extensions;
using Game.Areas;
using Game.Economy;
using Game.Modding;
using Game.Settings;
using Game.UI.Menu;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExtractAnywhere.Options
{
    [FileLocation("Mods_T3nak_ExtractAnywhere")]
    public class ExtractAnywhereOptions : ModSetting
    {
        public ExtractAnywhereOptions(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUIHidden]
        public CanExtractAnywhereOption CanExtractAnywhere { get; set; } = new();

        [SettingsUIHidden]
        public ExtractorRadiusOption ExtractionRadius { get; set; } = new();

        [SettingsUIHidden]
        public virtual int Version { get; set; }

        private void GetPageSection(AutomaticSettings.SettingPageData pageData, MapFeature feature)
        {
            string tabName = feature.ToString();

            bool didAddAny = false;

            if (CanExtractAnywhere.MapFeatureSupportedResources.TryGetValue(feature, out HashSet<Resource> resources))
            {
                didAddAny = true;

                foreach (Resource resource in resources)
                {
                    pageData[tabName].AddItem(new AutomaticSettings.SettingItemData
                    {
                        setting = this,
                        property = new AutomaticSettings.ManualProperty(
                            declaringType: typeof(ExtractAnywhereOptions),
                            propertyType: typeof(bool),
                            name: $"{resource}.{nameof(CanExtractAnywhere)}"
                        )
                        {
                            canRead = true,
                            canWrite = true,
                            getter = (options) => CanExtractAnywhere.CanResourceBeExtractedAnywhere(resource),
                            setter = (object options, object value) =>
                            {
                                // options is an instance of ExtractAnywhereOptions.
                                CanExtractAnywhere.ToggleResourceExtractionAnywhere(
                                    resource,
                                    value is bool isEnabled
                                        ? isEnabled
                                        : null
                                );
                            }
                        },
                        widgetType = AutomaticSettings.WidgetType.BoolToggle,
                        simpleGroup = nameof(CanExtractAnywhere)
                    });
                }

                pageData.AddGroup(nameof(CanExtractAnywhere));
                pageData.AddGroupToShowName(nameof(CanExtractAnywhere));
            }

            if (ExtractionRadius.IsMapFeatureSupported(feature))
            {
                didAddAny = true;

                pageData[tabName].AddItem(new AutomaticSettings.SettingItemData
                {
                    setting = this,
                    property = new AutomaticSettings.ManualProperty(
                        declaringType: typeof(ExtractAnywhereOptions),
                        propertyType: typeof(float),
                        name: $"{feature}.{nameof(ExtractionRadius)}"
                    )
                    {
                        attributes =
                        {
                            new SettingsUISliderAttribute
                            {
                                min = 0,
                                max = 10,
                                step = 0.1f,
                                unit = "floatSingleFraction"
                            }
                        },
                        canRead = true,
                        canWrite = true,
                        getter = (options) => ExtractionRadius.GetResourceExtractionRadius(feature),
                        setter = (object options, object value) =>
                        {
                            // options is an instance of ExtractAnywhereOptions.

                            ExtractionRadius.SetResourceExtractionRadius(
                                feature,
                                value is float radius
                                    ? (float)Math.Round(radius, 1)
                                    : float.TryParse(value.ToString(), out float parsedRadius)
                                        ? (float)Math.Round(parsedRadius, 1)
                                        : 300
                            );
                        }
                    },
                    widgetType = AutomaticSettings.WidgetType.FloatSlider,
                    simpleGroup = nameof(ExtractionRadius)
                });

                pageData.AddGroup(nameof(ExtractionRadius));
            }

            if (didAddAny)
            {
                pageData[tabName].AddItem(new AutomaticSettings.SettingItemData
                {
                    setting = this,
                    property = new AutomaticSettings.ManualProperty(
                        declaringType: typeof(ExtractAnywhereOptions),
                        propertyType: typeof(bool),
                        name: $"{feature}.Reset"
                    )
                    {
                        canRead = false,
                        canWrite = false,
                        attributes =
                        {
                            new SettingsUIButtonAttribute(),
                            new SettingsUIConfirmationAttribute()
                        },
                        setter = delegate (object obj, object value)
                        {
                            SetDefaultsForFeature(feature);
                            ApplyAndSave();
                        }
                    },
                    widgetType = AutomaticSettings.WidgetType.BoolButtonWithConfirmation,
                    simpleGroup = "Reset"
                });

                pageData.AddGroup("Reset");
            }
        }

        protected virtual void GenerateVersion()
        {
            Version++;
        }

        protected virtual void CanExtractAnywhereChanged(object sender, ExtractorOptionChangedEventArgs<Dictionary<Resource, bool>> e)
        {
            GenerateVersion();
        }

        protected virtual void ExtractionRadiusChanged(object sender, ExtractorOptionChangedEventArgs<Dictionary<MapFeature, float>> e)
        {
            GenerateVersion();
        }

        public override void SetDefaults()
        {
            Version = 0;

            CanExtractAnywhere.SetDefaults();
            CanExtractAnywhere.ValueChanged -= CanExtractAnywhereChanged;
            CanExtractAnywhere.ValueChanged += CanExtractAnywhereChanged;

            ExtractionRadius.SetDefaults();
            ExtractionRadius.ValueChanged -= ExtractionRadiusChanged;
            ExtractionRadius.ValueChanged += ExtractionRadiusChanged;
        }

        public virtual void SetDefaultsForFeature(MapFeature feature)
        {
            CanExtractAnywhere.SetDefaults(feature);
            ExtractionRadius.SetDefaults(feature);
        }

        public override AutomaticSettings.SettingPageData GetPageData(string id, bool addPrefix)
        {
            AutomaticSettings.SettingPageData pageData = base.GetPageData(id, addPrefix);

            pageData["General"]
                .FluentAddItem(new AutomaticSettings.SettingItemData()
                {
                    setting = this,
                    property = new AutomaticSettings.ManualProperty(
                        declaringType: typeof(ExtractAnywhereOptions),
                        propertyType: typeof(bool),
                        name: nameof(ExtractAnywhereOptionsManager.Instance.CanAnyExtractAnywhere)
                    )
                    {
                        canRead = true,
                        canWrite = true,
                        getter = (options) => ExtractAnywhereOptionsManager.CreateInstance(this).CanAnyExtractAnywhere,
                        setter = (object options, object value) => ExtractAnywhereOptionsManager.CreateInstance(this).CanAnyExtractAnywhere = value is bool isEnabled
                            ? isEnabled
                            : !ExtractAnywhereOptionsManager.Instance!.CanAnyExtractAnywhere
                    },
                    widgetType = AutomaticSettings.WidgetType.BoolToggle,
                    simpleGroup = nameof(ExtractAnywhereMod)
                })
                .FluentAddItem(new AutomaticSettings.SettingItemData()
                {
                    setting = this,
                    property = new AutomaticSettings.ManualProperty(
                        declaringType: typeof(ExtractAnywhereOptions),
                        propertyType: typeof(float),
                        name: nameof(ExtractAnywhereOptionsManager.MaxExtractionRadius)
                    )
                    {
                        attributes =
                            {
                                new SettingsUISliderAttribute
                                {
                                    min = 0,
                                    max = 10,
                                    step = 0.1f,
                                    unit = "floatSingleFraction"
                                }
                            },
                        canRead = false,
                        canWrite = false,
                        getter = (options) => ExtractAnywhereOptionsManager.CreateInstance(this).MaxExtractionRadius,
                        setter = (object options, object value) => ExtractAnywhereOptionsManager.CreateInstance(this).MaxExtractionRadius = value is float radius
                            ? (float)Math.Round(radius, 1)
                            : float.TryParse(value.ToString(), out float parsedRadius)
                                ? (float)Math.Round(parsedRadius, 1)
                                : 300
                    },
                    widgetType = AutomaticSettings.WidgetType.FloatSlider,
                    simpleGroup = nameof(ExtractAnywhereMod)
                })
                .FluentAddItem(new AutomaticSettings.SettingItemData
                {
                    setting = this,
                    property = new AutomaticSettings.ManualProperty(
                        declaringType: typeof(ExtractAnywhereOptions),
                        propertyType: typeof(bool),
                        name: "Reset"
                    )
                    {
                        canRead = false,
                        canWrite = true,
                        attributes =
                        {
                            new SettingsUIButtonAttribute(),
                            new SettingsUIConfirmationAttribute()
                        },
                        setter = delegate (object obj, object value)
                        {
                            SetDefaults();
                            ApplyAndSave();
                        }
                    },
                    widgetType = AutomaticSettings.WidgetType.BoolButtonWithConfirmation,
                    simpleGroup = nameof(ExtractAnywhereMod)
                });

            pageData.AddGroup(nameof(ExtractAnywhereMod));

            foreach (MapFeature feature in Enum.GetValues(typeof(MapFeature)))
            {
                GetPageSection(pageData, feature);
            }

            return pageData;
        }
    }
}
