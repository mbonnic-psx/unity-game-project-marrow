namespace ShaderToolboxPro.URP
{
    using UnityEditor;

    public sealed class StochasticLitShaderGUI : DefaultLitShaderGUI
    {
        protected override string bannerTexturePath { get { return "StochasticLitBanner"; } }
        protected override string bannerFallbackText { get { return "Stochastic Lit"; } }
        protected override string headerText { get { return "A PBR shader effect which uses random UV offsets to break up texture tiling."; } }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            base.OnGUI(materialEditor, properties);

            materialEditor.serializedObject.ApplyModifiedProperties();
        }
    }
}
