namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public class HeatDistortionShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty heatOriginPoint = new("_HeatOriginPoint", "Heat Origin Point", "The central point (in world space) from which the heat spreads.");
        private ToolboxProperty heatFalloff = new("_HeatFalloff", "Heat Falloff", "How swiftly the heat distortion strength is reduced at increasing distance from the origin.");
        private ToolboxProperty distortionMap = new("_DistortionMap", "Distortion Map", "Flow map which controls the heat distortion pattern.");
        private ToolboxProperty distortionStrength = new("_DistortionStrength", "Distortion Strength", "How strongly the opaque texture gets distorted by the distortion map.");
        private ToolboxProperty distortionVelocity = new("_DistortionVelocity", "Distortion Velocity", "How quickly the distortion map scrolls over the screen.");
        private ToolboxProperty distortionTiling = new("_DistortionTiling", "Distortion Tiling", "How many times the distortion map is tiled across the screen.");
        private ToolboxProperty cameraTextureMode = new("_CAMERA_TEXTURE_MODE", "Camera Texture Mode", "Should this effect use the _CameraOpaqueTexture, or the _CameraTransparentTexture?" +
            "\n\nWarning: Transparent mode requires the experimental Copy Transparent Texture renderer feature from Shader Toolbox, which incurs a performance penalty. Consult the documentation for usage details.");

        protected override string bannerTexturePath { get { return "HeatDistortionBanner"; } }
        protected override string bannerFallbackText { get { return "Heat Distortion"; } }
        protected override string headerText { get { return "An effect which distorts the camera opaque texture according to a flow map texture."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            heatOriginPoint.prop = FindProperty(heatOriginPoint.name, props, true);
            heatFalloff.prop = FindProperty(heatFalloff.name, props, true);
            distortionMap.prop = FindProperty(distortionMap.name, props, true);
            distortionStrength.prop = FindProperty(distortionStrength.name, props, true);
            distortionVelocity.prop = FindProperty(distortionVelocity.name, props, true);
            distortionTiling.prop = FindProperty(distortionTiling.name, props, true);
            cameraTextureMode.prop = FindProperty(cameraTextureMode.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Heat Distortion Properties"), 1u << 0, DrawHeatOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 1, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeatOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(heatOriginPoint.prop, heatOriginPoint.info);
            materialEditor.ShaderProperty(heatFalloff.prop, heatFalloff.info);
            materialEditor.ShaderProperty(distortionMap.prop, distortionMap.info);
            materialEditor.ShaderProperty(distortionStrength.prop, distortionStrength.info);
            materialEditor.ShaderProperty(distortionVelocity.prop, distortionVelocity.info);
            materialEditor.ShaderProperty(distortionTiling.prop, distortionTiling.info);
            materialEditor.ShaderProperty(cameraTextureMode.prop, cameraTextureMode.info);
        }
    }
}

