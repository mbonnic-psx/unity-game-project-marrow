namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class PortalCardShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty fadeThresholds = new("_FadeThresholds", "Fade Thresholds", "Blend the color based on the distance between the mesh and the depth of the last rendered object at the same screen coordinate." +
            "\nPixels with distance below the x-value get tinted with Fog Start Color, and pixels with distance over the y-value get tinted with Fog End Color, with a smooth transition in between.");
        private ToolboxProperty distanceThresholds = new("_DistanceThresholds", "Distance Thresholds", "Fades the alpha of the mesh based on the distance between the camera and the mesh pivot point. The mesh becomes invisible at distances over the y-value, and is at full alpha at distances below the x-value.");
        private ToolboxProperty edgeFade = new("_EdgeFade", "Edge Fade", "Fades the edges of the mesh based on UV corodinates, with separate values for the U- and V-axes.");
        private ToolboxProperty fogStartColor = new("_FogStartColor", "Fog Start Color", "Color of the portal fog close to the mesh.");
        private ToolboxProperty fogEndColor = new("_FogEndColor", "Fog End Color", "Color of the portal fog far away from the mesh.");

        protected override string bannerTexturePath { get { return "PortalCardBanner"; } }
        protected override string bannerFallbackText { get { return "Portal Card"; } }
        protected override string headerText { get { return "A shader effect which fades colors based on depth-distance from scene elements, and fades alpha based on camera distance."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            fadeThresholds.prop = FindProperty(fadeThresholds.name, props, true);
            distanceThresholds.prop = FindProperty(distanceThresholds.name, props, true);
            edgeFade.prop = FindProperty(edgeFade.name, props, true);
            fogStartColor.prop = FindProperty(fogStartColor.name, props, true);
            fogEndColor.prop = FindProperty(fogEndColor.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Portal Card Properties"), 1u << 0, DrawPortalCardOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 1, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawPortalCardOptions(Material material)
        {
            EditorGUILayout.Space(3);

            fadeThresholds.prop.vectorValue = EditorGUILayout.Vector2Field(fadeThresholds.info, fadeThresholds.prop.vectorValue);
            distanceThresholds.prop.vectorValue = EditorGUILayout.Vector2Field(distanceThresholds.info, distanceThresholds.prop.vectorValue);
            edgeFade.prop.vectorValue = EditorGUILayout.Vector2Field(edgeFade.info, edgeFade.prop.vectorValue);

            materialEditor.ShaderProperty(fogStartColor.prop, fogStartColor.info);
            materialEditor.ShaderProperty(fogEndColor.prop, fogEndColor.info);
        }
    }
}
