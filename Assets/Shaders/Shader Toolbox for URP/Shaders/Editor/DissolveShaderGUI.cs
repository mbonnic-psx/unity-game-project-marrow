namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class DissolveShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty noiseScale = new("_NoiseScale", "Noise Scale", "Scale of the noise pattern used for the glowing edge.");
        private ToolboxProperty noiseStrength = new("_NoiseStrength", "Noise Strength", "How strongly the noise pattern raises or lowers the glowing edge.");
        private ToolboxProperty originType = new("_ORIGIN_TYPE", "Origin Type", "Should the effect dissolve objects on one side of a plane, or dissolve based on distance from a point in space?");
        private ToolboxProperty cutoffPoint = new("_CutoffPoint", "Cutoff Point", "In Plane mode: Parameter denoting a point on the plane." +
            "\nIn Point mode: Origin point of the dissolve effect.");
        private ToolboxProperty cutoffDirection = new("_CutoffDirection", "Cutoff Direction", "Parameter denoting the direction the plane faces.");
        private ToolboxProperty cutoffHeight = new("_CutoffHeight", "Cutoff Height", "The distance from the origin point or the plane before pixels start being cutoff.");
        private ToolboxProperty flipDirection = new("_FlipDirection", "Flip Direction", "When enabled, the dissolve acts in the opposite direction from the plane or origin point.");
        private ToolboxProperty glowColor = new("_GlowColor", "Glow Color", "Color of the glowing edge. Can be emissive.");
        private ToolboxProperty glowThickness = new("_GlowThickness", "Glow Thickness", "Thickness of the glowing edge.");
        private ToolboxProperty useWorldSpace = new("_UseWorldSpace", "Use World Space", "Should the plane/origin values be specified in world or object space?");
        private ToolboxProperty useEmission = new("_UseEmission", "Use Emission", "Should the glow color be output to the Emission or Base Color channel?");

        protected override string bannerTexturePath { get { return "DissolveBanner"; } }
        protected override string bannerFallbackText { get { return "Dissolve"; } }
        protected override string headerText { get { return "A dissolve effect which can use a plane defined in world space to perform the cutoff."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            noiseScale.prop = FindProperty(noiseScale.name, props, true);
            noiseStrength.prop = FindProperty(noiseStrength.name, props, true);
            originType.prop = FindProperty(originType.name, props, true);
            cutoffPoint.prop = FindProperty(cutoffPoint.name, props, true);
            cutoffDirection.prop = FindProperty(cutoffDirection.name, props, true);
            cutoffHeight.prop = FindProperty(cutoffHeight.name, props, true);
            flipDirection.prop = FindProperty(flipDirection.name, props, true);
            glowColor.prop = FindProperty(glowColor.name, props, true);
            glowThickness.prop = FindProperty(glowThickness.name, props, true);
            useWorldSpace.prop = FindProperty(useWorldSpace.name, props, true);
            useEmission.prop = FindProperty(useEmission.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Dissolve Properties"), 1u << 2, DrawDissolveOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawDissolveOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(noiseScale.prop, noiseScale.info);
            materialEditor.ShaderProperty(noiseStrength.prop, noiseStrength.info);
            materialEditor.ShaderProperty(originType.prop, originType.info);

            var isPlaneEnabled = material.IsKeywordEnabled("_ORIGIN_TYPE_PLANE");

            materialEditor.ShaderProperty(cutoffPoint.prop, cutoffPoint.info);

            if(isPlaneEnabled)
            {
                materialEditor.ShaderProperty(cutoffDirection.prop, cutoffDirection.info);
            }
                
            materialEditor.ShaderProperty(cutoffHeight.prop, cutoffHeight.info);
            materialEditor.ShaderProperty(flipDirection.prop, flipDirection.info);
            materialEditor.ShaderProperty(glowColor.prop, glowColor.info);
            materialEditor.ShaderProperty(glowThickness.prop, glowThickness.info);
            materialEditor.ShaderProperty(useWorldSpace.prop, useWorldSpace.info);
            materialEditor.ShaderProperty(useEmission.prop, useEmission.info);
        }
    }
}
