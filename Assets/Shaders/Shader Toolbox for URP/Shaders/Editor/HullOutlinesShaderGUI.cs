namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEngine;

    public class HullOutlinesShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty outlineColor = new("_OutlineColor", "Outline Color", "Base color of the outlines.");
        private ToolboxProperty outlineThickness = new("_OutlineThickness", "Outline Thickness", "Thickness of the outlines in world units.");

        protected override string bannerTexturePath { get { return "HullOutlineBanner"; } }
        protected override string bannerFallbackText { get { return "Inverted-hull Outlines"; } }
        protected override string headerText { get { return "An effect which renders the object inside out and expanded along vertex normals. Best used in the second material slot on a renderer."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            outlineColor.prop = FindProperty(outlineColor.name, props, true);
            outlineThickness.prop = FindProperty(outlineThickness.name, props, true);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Hull Outline Properties"), 1u << 0, DrawOutlineOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 1, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }

        private void DrawOutlineOptions(Material material)
        {
            EditorGUILayout.Space(3);

            materialEditor.ShaderProperty(outlineColor.prop, outlineColor.info);
            materialEditor.ShaderProperty(outlineThickness.prop, outlineThickness.info);
        }
    }
}
