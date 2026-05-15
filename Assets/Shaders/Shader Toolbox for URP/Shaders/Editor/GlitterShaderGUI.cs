namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class GlitterShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty noiseScale = new("_NoiseScale", "Noise Scale", "How frequently the Voronoi noise pattern is tiled across the surface of the mesh.");
        private ToolboxProperty noiseRotationSpeed = new("_NoiseRotation", "Noise Rotation Speed", "How quickly the random values increase over time, rotating the random vector direction that is used for glint calculations.");
        private ToolboxProperty glitterOffset = new("_GlitterOffset", "Glitter Offset", "A modifier which determines how likely is is that a glitter particle actually reflects light.");
        private ToolboxProperty sparkliness = new("_Sparkliness", "Sparkliness", "Higher values mean glitter particles reflect with a narrower angle range, causing the object to look 'more sparkly' when moving around it.");
        private ToolboxProperty glitterColor = new("_GlitterColor", "Glitter Color", "Color of the glitter particles at one end of the generated noise range.");
        private ToolboxProperty glitterColor2 = new("_GlitterColor2", "Glitter Color 2", "Color of the glitter particles at the other end of the generated noise range.");
        private ToolboxProperty fresnelPower = new("_FresnelPower", "Fresnel Power", "Power applied to the Fresnel calculations. Higher values mean thinner Fresnel highlights around the rim of the object.");
        private ToolboxProperty fresnelColor = new("_FresnelColor", "Fresnel Color", "Color multiplier for the Fresnel lighting.");
        private ToolboxProperty spotOffset = new("_SpotOffset", "Spot Offset", "An angle offset applied to the Voronoi pattern generator. A value of zero results in an even grid of glitter particles.");
        private ToolboxProperty spotThresholds = new("_SpotThresholds", "Spot Thresholds", "Smoothstep thresholds values applied to the glitter particles. The second value should be at least as large as the first. Values above zero typically result in circle shapes, whereas values near zero will give you jagged glitter particle shapes.");

        protected override string bannerTexturePath { get { return "GlitterBanner"; } }
        protected override string bannerFallbackText { get { return "Glitter"; } }
        protected override string headerText { get { return "A glitter effect which generates random vectors along a Voronoi noise pattern to produce random reflections."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            noiseScale.prop = FindProperty(noiseScale.name, props, true);
            noiseRotationSpeed.prop = FindProperty(noiseRotationSpeed.name, props, true);
            glitterOffset.prop = FindProperty(glitterOffset.name, props, true);
            sparkliness.prop = FindProperty(sparkliness.name, props, true);
            glitterColor.prop = FindProperty(glitterColor.name, props, true);
            glitterColor2.prop = FindProperty(glitterColor2.name, props, true);
            fresnelPower.prop = FindProperty(fresnelPower.name, props, true);
            fresnelColor.prop = FindProperty(fresnelColor.name, props, true);
            spotOffset.prop = FindProperty(spotOffset.name, props, true);
            spotThresholds.prop = FindProperty(spotThresholds.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Glitter Properties"), 1u << 2, DrawGlitterOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawGlitterOptions(Material material)
        {
            EditorGUILayout.Space(3);

            noiseScale.prop.vectorValue = EditorGUILayout.Vector2Field(noiseScale.info, noiseScale.prop.vectorValue);

            materialEditor.ShaderProperty(noiseRotationSpeed.prop, noiseRotationSpeed.info);
            materialEditor.ShaderProperty(glitterOffset.prop, glitterOffset.info);
            materialEditor.ShaderProperty(sparkliness.prop, sparkliness.info);
            materialEditor.ShaderProperty(glitterColor.prop, glitterColor.info);
            materialEditor.ShaderProperty(glitterColor2.prop, glitterColor2.info);
            materialEditor.ShaderProperty(fresnelPower.prop, fresnelPower.info);
            materialEditor.ShaderProperty(fresnelColor.prop, fresnelColor.info);
            materialEditor.ShaderProperty(spotOffset.prop, spotOffset.info);

            spotThresholds.prop.vectorValue = EditorGUILayout.Vector2Field(spotThresholds.info, spotThresholds.prop.vectorValue);
        }
    }
}
