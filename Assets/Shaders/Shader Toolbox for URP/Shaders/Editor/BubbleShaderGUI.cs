namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class BubbleShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty colorRampTexture = new("_ColorRampTexture", "Color Ramp Texture", "A thin horizontal strip texture with iridescent color values." + 
            "\nIf no texture is set, a color ramp will be generated in the shader");
        private ToolboxProperty useColorRampTexture = new("_USE_COLOR_RAMP_TEXTURE", "Use Color Ramp Texture", "");
        private ToolboxProperty fresnelPower = new("_FresnelPower", "Fresnel Power", "The thickness of the bubble layer. Higher values cover less surface.");
        private ToolboxProperty fresnelNoiseStrength = new("_FresnelNoiseStrength", "Fresnel Noise Strength", "How strongly noise affects the bubble layer colors");
        private ToolboxProperty fresnelNoiseScale = new("_FresnelNoiseScale", "Fresnel Noise Scale", "How detailed the noise applied to the fresnel layer should be.");
        private ToolboxProperty iridescentStrength = new("_IridescentStrength", "Iridescent Strength", "How visible the bubble layer is.");
        private ToolboxProperty iridescentFlowDirection = new("_IridescentFlowDirection", "Iridescent Flow Direction", "Which direction the Fresnel noise should be pointing.");
        private ToolboxProperty iridescentFlowSpeed = new("_IridescentFlowSpeed", "Iridescent Flow Speed", "How quickly the noise scrolls in the flow direction.");
        private ToolboxProperty refractiveIndex = new("_RefractiveIndex", "Refractive Index", "How strongly light should bend through the object. Higher values mean lower bending.");
        private ToolboxProperty useEmission = new("_UseEmission", "Use Emission", "Should the bubble layer be applied to emission or base color?");
        private ToolboxProperty cameraTextureMode = new("_CAMERA_TEXTURE_MODE", "Camera Texture Mode", "Should this effect use the _CameraOpaqueTexture, or the _CameraTransparentTexture?\n\n" +
            "Warning: Transparent mode requires the experimental Copy Transparent Texture renderer feature from Shader Toolbox, which incurs a performance penalty. Consult the documentation for usage details.");

        protected override string bannerTexturePath { get { return "BubbleBanner"; } }
        protected override string bannerFallbackText { get { return "Bubbles"; } }
        protected override string headerText { get { return "An iridescent, transparent bubble with customizable color ramps and animated flow properties."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            colorRampTexture.prop = FindProperty(colorRampTexture.name, props, true);
            useColorRampTexture.prop = FindProperty(useColorRampTexture.name, props, true);
            fresnelPower.prop = FindProperty(fresnelPower.name, props, true);
            fresnelNoiseStrength.prop = FindProperty(fresnelNoiseStrength.name, props, true);
            fresnelNoiseScale.prop = FindProperty(fresnelNoiseScale.name, props, true);
            iridescentStrength.prop = FindProperty(iridescentStrength.name, props, true);
            iridescentFlowDirection.prop = FindProperty(iridescentFlowDirection.name, props, true);
            iridescentFlowSpeed.prop = FindProperty(iridescentFlowSpeed.name, props, true);
            refractiveIndex.prop = FindProperty(refractiveIndex.name, props, true);
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
                materialScopeList.RegisterHeaderScope(new GUIContent("Bubble Properties"), 1u << 2, DrawBubbleOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawBubbleOptions(Material material)
        {
            EditorGUILayout.Space(3);
            
            materialEditor.TexturePropertySingleLine(colorRampTexture.info, colorRampTexture.prop);
            var texture = material.GetTexture(colorRampTexture.name);

            if (texture == null)
            {
                material.SetFloat(useColorRampTexture.name, 0.0f);
                material.DisableKeyword(useColorRampTexture.name);
            }
            else
            {
                GUILayout.Space(5);
                var rect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth + EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.DrawPreviewTexture(rect, texture);
                GUILayout.Space(5);

                material.SetFloat(useColorRampTexture.name, 1.0f);
                material.EnableKeyword(useColorRampTexture.name);
            }

            materialEditor.ShaderProperty(fresnelPower.prop, fresnelPower.info);
            materialEditor.ShaderProperty(fresnelNoiseStrength.prop, fresnelNoiseStrength.info);
            materialEditor.ShaderProperty(fresnelNoiseScale.prop, fresnelNoiseScale.info);
            materialEditor.ShaderProperty(iridescentStrength.prop, iridescentStrength.info);
            materialEditor.ShaderProperty(iridescentFlowDirection.prop, iridescentFlowDirection.info);
            materialEditor.ShaderProperty(iridescentFlowSpeed.prop, iridescentFlowSpeed.info);
            materialEditor.ShaderProperty(refractiveIndex.prop, refractiveIndex.info);
            materialEditor.ShaderProperty(useEmission.prop, useEmission.info);
            materialEditor.ShaderProperty(cameraTextureMode.prop, cameraTextureMode.info);
        }
    }
}
