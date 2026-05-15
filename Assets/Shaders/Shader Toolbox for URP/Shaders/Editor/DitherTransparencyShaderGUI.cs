namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class DitherTransparencyShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty useDitherTexture = new("_USE_DITHER_TEXTURE", "Use Dither Texture", "Should the shader use a texture to determine dither thresholds? Otherwise, a Bayer matrix is generated in-shader.");
        private ToolboxProperty ditherTexture = new("_DitherTexture", "Dither Texture", "A texture to use for the dither pattern. The red channel is sampled.");
        private ToolboxProperty ditherSize = new("_DitherSize", "Dither Scale", "Size of the dithering pattern. Note that integer values work best.");
        private ToolboxProperty opacity = new("_Opacity", "Opacity", "Multiplier applied to the object's alpha.");

        protected override string bannerTexturePath { get { return "DitherBanner"; } }
        protected override string bannerFallbackText { get { return "Dither Transparency"; } }
        protected override string headerText { get { return "A faux-transparency effect which uses alpha clipping along a Bayer matrix pattern."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            useDitherTexture.prop = FindProperty(useDitherTexture.name, props, true);
            ditherTexture.prop = FindProperty(ditherTexture.name, props, true);
            ditherSize.prop = FindProperty(ditherSize.name, props, true);
            opacity.prop = FindProperty(opacity.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Dithering Properties"), 1u << 2, DrawDitherOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawDitherOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(useDitherTexture.prop, useDitherTexture.info);

            var useTexture = material.IsKeywordEnabled("_USE_DITHER_TEXTURE");

            if(useTexture)
            {
                materialEditor.ShaderProperty(ditherTexture.prop, ditherTexture.info);
            }

            materialEditor.ShaderProperty(ditherSize.prop, ditherSize.info);
            materialEditor.ShaderProperty(opacity.prop, opacity.info);
        }
    }
}
