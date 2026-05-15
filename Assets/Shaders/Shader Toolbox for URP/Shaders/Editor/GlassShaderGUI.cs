namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class GlassShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty refractiveIndex = new("_RefractiveIndex", "Refractive Index", "How strongly light should bend through the object. Higher values mean lower bending.");
        private ToolboxProperty glassStrength = new("_GlassStrength", "Glass Strength", "Proportion of final base color that uses the scene color.");
        private ToolboxProperty fresnelPower = new("_FresnelPower", "Fresnel Power", "Thickness of the Fresnel light. Higher values cover less surface.");
        private ToolboxProperty fresnelColor = new("_FresnelColor", "Fresnel Color", "Color of the Fresnel highlights.");
        private ToolboxProperty useEmission = new("_UseEmission", "Use Emission", "Should the Fresnel layer be applied to emission or base color?");
        private ToolboxProperty cameraTextureMode = new("_CAMERA_TEXTURE_MODE", "Camera Texture Mode", "Should this effect use the _CameraOpaqueTexture, or the _CameraTransparentTexture?" +
            "\n\nWarning: Transparent mode requires the experimental Copy Transparent Texture renderer feature from Shader Toolbox, which incurs a performance penalty. Consult the documentation for usage details.");

        protected override string bannerTexturePath { get { return "GlassBanner"; } }
        protected override string bannerFallbackText { get { return "Glass"; } }
        protected override string headerText { get { return "A glass effect which samples the camera opaque texture for screen-space refraction."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            refractiveIndex.prop = FindProperty(refractiveIndex.name, props, true);
            glassStrength.prop = FindProperty(glassStrength.name, props, true);
            fresnelPower.prop = FindProperty(fresnelPower.name, props, true);
            fresnelColor.prop = FindProperty(fresnelColor.name, props, true);
            useEmission.prop = FindProperty(useEmission.name, props, true);
            cameraTextureMode.prop = FindProperty(cameraTextureMode.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Glass Properties"), 1u << 2, DrawGlassOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawGlassOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(refractiveIndex.prop, refractiveIndex.info);
            materialEditor.ShaderProperty(glassStrength.prop, glassStrength.info);
            materialEditor.ShaderProperty(fresnelPower.prop, fresnelPower.info);
            materialEditor.ShaderProperty(fresnelColor.prop, fresnelColor.info);
            materialEditor.ShaderProperty(useEmission.prop, useEmission.info);
            materialEditor.ShaderProperty(cameraTextureMode.prop, cameraTextureMode.info);
        }
    }
}
