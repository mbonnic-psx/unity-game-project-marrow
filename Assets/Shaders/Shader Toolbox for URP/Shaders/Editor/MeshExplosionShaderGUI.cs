namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public sealed class MeshExplosionShaderGUI : DefaultLitShaderGUI
    {
        private ToolboxProperty expansionMode = new("_EXPANSION_MODE", "Expansion Mode", "How to expand the mesh.\n" +
            "\nNormals: expand along the vertex normal.\n" +
            "\nOffset: expand along the offset vector from the vertex position to the explosion origin.\n" +
            "\nColors: expand along the offset vector from the position data baked into the vertex colors to the explosion origin.\n" +
            "\nNote that Colors mode works well when combined with the Bake Face Pos to Vertex Color tool.");
        private ToolboxProperty explosionOrigin = new("_ExplosionOriginPoint", "Explosion Origin Point", "World origin point that explosions move away from.");
        private ToolboxProperty explosionDistance = new("_ExplosionDistance", "Explosion Distance", "How far the parts of the mesh extend from the center along the normal vector.");
        private ToolboxProperty debrisShrink = new("_DebrisShrinkSpeed", "Debris Shrink Speed", "How rapidly each mesh fragment decreases in size at higher distances.");
        private ToolboxProperty gravity = new("_Gravity", "Gravity", "How rapidly each mesh fragment falls to the ground at higher distances.");
        private ToolboxProperty randomOffsetRange = new("_RandomOffsetRange", "Random Offset Range", "Apply a random multiplier to the explosion distance between these values." +
            "\n\nTry and keep this value low, since higher values tend to spaghettify the mesh at high explosion distances.");

        protected override string bannerTexturePath { get { return "MeshExplosionBanner"; } }
        protected override string bannerFallbackText { get { return "Mesh Explosion"; } }
        protected override string headerText { get { return "A vertex displacement effect with configurable explosion origin settings."; } }

        private enum MeshExplosionMode
        {
            NORMALS,
            ORIGIN,
            COLORS
        }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            expansionMode.prop = FindProperty(expansionMode.name, props, true);
            explosionOrigin.prop = FindProperty(explosionOrigin.name, props, true);
            explosionDistance.prop = FindProperty(explosionDistance.name, props, true);
            debrisShrink.prop = FindProperty(debrisShrink.name, props, true);
            gravity.prop = FindProperty(gravity.name, props, true);
            randomOffsetRange.prop = FindProperty(randomOffsetRange.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Explosion Properties"), 1u << 2, DrawExplosionOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 3, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawExplosionOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(expansionMode.prop, expansionMode.info);

            if(material.GetFloat(expansionMode.name) > 0)
            {
                materialEditor.ShaderProperty(explosionOrigin.prop, explosionOrigin.info);
            }

            materialEditor.ShaderProperty(explosionDistance.prop, explosionDistance.info);
            materialEditor.ShaderProperty(debrisShrink.prop, debrisShrink.info);
            materialEditor.ShaderProperty(gravity.prop, gravity.info);
            materialEditor.ShaderProperty(randomOffsetRange.prop, randomOffsetRange.info);
        }
    }
}
