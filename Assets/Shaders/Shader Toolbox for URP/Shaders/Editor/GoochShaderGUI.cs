namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public class GoochShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty warmColor = new("_WarmColor", "Warm Color", "Color applied to lit side of the object.");
        private ToolboxProperty coldColor = new("_ColdColor", "Cold Color", "Color applied to unlit side of the object.");
        private ToolboxProperty temperatureOffset = new("_TemperatureOffset", "Temperature Offset", "Skews the temperature towards cold (values below 1) or towards hot (values above 1).");
        private ToolboxProperty specularPower = new("_SpecularPower", "Specular Power", "Controls size of the specular highlight. Larger values result in smaller highlights.");
        private ToolboxProperty specularColor = new("_SpecularColor", "Specular Color", "Color of the specular highlights.");
        private ToolboxProperty useHCLColorSpace = new("_USE_HCL_COLOR_SPACE", "Use HCL Color Space", "Use HCL color space to blend colors." +
            "\nBlending with HCL color space better preserves the luminosity of the input colors over RGB blending." +
            "\nHCL color blending is slightly more expensive due to RGB->HCL conversion.");

        protected override string bannerTexturePath { get { return "GoochBanner"; } }
        protected override string bannerFallbackText { get { return "Gooch"; } }
        protected override string headerText { get { return "A non-photorealistic rendering technique used in computer-aided design which emphasizes normal directions."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            warmColor.prop = FindProperty(warmColor.name, props, true);
            coldColor.prop = FindProperty(coldColor.name, props, true);
            temperatureOffset.prop = FindProperty(temperatureOffset.name, props, true);
            specularPower.prop = FindProperty(specularPower.name, props, true);
            specularColor.prop = FindProperty(specularColor.name, props, true);
            useHCLColorSpace.prop = FindProperty(useHCLColorSpace.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Gooch Properties"), 1u << 0, DrawGoochOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 1, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawGoochOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(warmColor.prop, warmColor.info);
            materialEditor.ShaderProperty(coldColor.prop, coldColor.info);
            materialEditor.ShaderProperty(temperatureOffset.prop, temperatureOffset.info);
            materialEditor.ShaderProperty(specularPower.prop, specularPower.info);
            materialEditor.ShaderProperty(specularColor.prop, specularColor.info);
            materialEditor.ShaderProperty(useHCLColorSpace.prop, useHCLColorSpace.info);
        }
    }
}
