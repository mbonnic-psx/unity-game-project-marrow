namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEditor.Rendering.Universal.ShaderGUI;
    using UnityEngine;

    public class DefaultLitShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty baseColor = new("_BaseColor", "Base Color", "Albedo color applied to entire mesh.");
        private ToolboxProperty baseTex = new("_BaseMap", "Base Texture", "Albedo texture applied to entire mesh.");
        private ToolboxProperty workflowMode = new("_WorkflowMode", "Workflow Mode", "");
        private ToolboxProperty metallic = new("_Metallic", "Metallic", "How metallic the surface should be." +
            "\n1 represents a metal, and 0 represents a non-metal." +
            "\nVery few objects in the real world use values around 0.5.");
        private ToolboxProperty metallicTex = new("_MetallicGlossMap");
        private ToolboxProperty specularColor = new("_SpecColor", "Specular Color", "What color should be used for specular highlights.");
        private ToolboxProperty specularTex = new("_SpecGlossMap");
        private ToolboxProperty smoothness = new("_Smoothness", "Smoothness", "How smooth the surface of the object should be." +
            "\n1 reprsents a highly polished surface. 0 represents a very rough or matter surface.");
        private ToolboxProperty smoothnessTex = new("_SmoothnessMap");
        private ToolboxProperty convertFromRoughness = new("_ConvertFromRoughness", "Convert From Roughness", "Does this material use a roughness texture instead of smoothness?");
        private ToolboxProperty normalStrength = new("_BumpScale", "Normal Map", "Normal map modifies the surface normals for finer lighting detail." +
            "\n1 represents 'standard' strength.");
        private ToolboxProperty normalMap = new("_BumpMap");
        private ToolboxProperty heightmapStrength = new("_Parallax", "Heightmap", "A heightmap can be used to 'fake' raised and lower sections on the surface.");
        private ToolboxProperty heightmapTex = new("_ParallaxMap");
        private ToolboxProperty occlusionStrength = new("_OcclusionStrength", "Ambient Occlusion", "Amount of ambient occlusion falling on the surface." +
            "\n1 represents a fully lit part of the surface, while 0 means a fully shadowed area.");
        private ToolboxProperty occlusionTex = new("_OcclusionMap");
        private ToolboxProperty emissionColor = new("_EmissionColor", "Emission Color", "The amount of emissive light to use on the surface." +
            "\nWhereas Base Color is influenced by scene lighting, emissive color is visible regardless of whether the object is in shadow.");
        private ToolboxProperty emissionTex = new("_EmissionMap");

        private ToolboxProperty receiveShadows = new("_ReceiveShadows", "Receive Shadows", "Toggle whether to render realtime shadows from other objects influenced by the scene lights.");
        private ToolboxProperty alphaClip = new("_AlphaClip", "Alpha Clip", "Should this object use alpha clipping?");
        private ToolboxProperty alphaClipThreshold = new("_Cutoff", "Threshold", "Pixels with an alpha value below this threshold are culled.");

        private const string cullName = "_Cull";
        private const string cullLabel = "Render Face";
        private const string cullTooltip = "Choose which sides of the mesh faces to render.";

        private const string blendModeName = "_Blend";
        private const string blendModeLabel = "Blend Mode";
        private const string blendModeTooltip = "How Unity should blend this mesh with previously drawn objects.";

        private const string surfaceTypeName = "_Surface";
        private const string surfaceTypeLabel = "Surface Type";
        private const string surfaceTypeTooltip = "Whether the mesh is rendered opaque or transparent.";

        private const string zWriteName = "_ZWrite";

        private bool shouldRenderMetallic = false;
        private bool shouldRenderSpecular = false;

        protected override string bannerTexturePath { get { return "DefaultLitBanner"; } }
        protected override string bannerFallbackText { get { return "Default Lit"; } }
        protected override string headerText { get { return "A PBR shader effect, just like URP's Lit shader."; } }

        protected override void FindProperties(MaterialProperty[] props)
        {
            baseColor.prop = FindProperty(baseColor.name, props, true);
            baseTex.prop = FindProperty(baseTex.name, props, true);
            workflowMode.prop = FindProperty(workflowMode.name, props, true);
            metallic.prop = FindProperty(metallic.name, props, true);
            metallicTex.prop = FindProperty(metallicTex.name, props, true);
            specularColor.prop = FindProperty(specularColor.name, props, true);
            specularTex.prop = FindProperty(specularTex.name, props, true);
            smoothness.prop = FindProperty(smoothness.name, props, true);
            smoothnessTex.prop = FindProperty(smoothnessTex.name, props, true);
            convertFromRoughness.prop = FindProperty(convertFromRoughness.name, props, true);
            normalMap.prop = FindProperty(normalMap.name, props, true);
            normalStrength.prop = FindProperty(normalStrength.name, props, true);
            heightmapStrength.prop = FindProperty(heightmapStrength.name, props, true);
            heightmapTex.prop = FindProperty(heightmapTex.name, props, true);
            occlusionStrength.prop = FindProperty(occlusionStrength.name, props, true);
            occlusionTex.prop = FindProperty(occlusionTex.name, props, true);
            emissionColor.prop = FindProperty(emissionColor.name, props, true);
            emissionTex.prop = FindProperty(emissionTex.name, props, true);

            alphaClip.prop = FindProperty(alphaClip.name, props, false);
            alphaClipThreshold.prop = FindProperty(alphaClipThreshold.name, props, true);
            receiveShadows.prop = FindProperty(receiveShadows.name, props, false);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);

            if (firstTimeOpen)
            {
                materialScopeList.RegisterHeaderScope(new GUIContent("Surface Options"), 1u << 0, DrawSurfaceOptions);
                materialScopeList.RegisterHeaderScope(new GUIContent("Lit Properties"), 1u << 1, DrawLitProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 2, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);
        }

        protected void DrawSurfaceOptions(Material material)
        {
            EditorGUILayout.Space(3);

            var surfaceTypeValue = (SurfaceType)material.GetFloat(surfaceTypeName);
            var renderFaceValue = (RenderFace)material.GetFloat(cullName);
            var blendModeValue = (BlendMode)material.GetFloat(blendModeName);
            var workflowModeValue = (LitGUI.WorkflowMode)material.GetFloat(workflowMode.name);

            shouldRenderMetallic = (metallicTex.prop != null);
            shouldRenderSpecular = (specularTex.prop != null);

            // Show the workflow mode only if it exists and there is actually a choice between both.
            if (workflowMode.prop != null && metallicTex.prop != null && specularTex.prop != null)
            {
                EditorGUI.BeginChangeCheck();
                {
                    workflowModeValue = (LitGUI.WorkflowMode)EditorGUILayout.EnumPopup(workflowMode.info, workflowModeValue);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(material, "Modify Workflow Mode");
                    material.SetFloat(workflowMode.name, (float)workflowModeValue);

                    if (workflowModeValue == LitGUI.WorkflowMode.Specular)
                    {
                        material.EnableKeyword("_SPECULAR_SETUP");
                    }
                    else
                    {
                        material.DisableKeyword("_SPECULAR_SETUP");
                    }

                    EditorUtility.SetDirty(material);
                }

                shouldRenderMetallic = (workflowModeValue == LitGUI.WorkflowMode.Metallic);
                shouldRenderSpecular = (workflowModeValue == LitGUI.WorkflowMode.Specular);
            }

            // Display opaque/transparent options.
            bool surfaceTypeChanged = false;
            EditorGUI.BeginChangeCheck();
            {
                surfaceTypeValue = (SurfaceType)EditorGUILayout.EnumPopup(new GUIContent(surfaceTypeLabel, surfaceTypeTooltip), surfaceTypeValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(material, "Modify Surface Type");
                surfaceTypeChanged = true;
            }

            // Display culling options.
            EditorGUI.BeginChangeCheck();
            {
                renderFaceValue = (RenderFace)EditorGUILayout.EnumPopup(new GUIContent(cullLabel, cullTooltip), renderFaceValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(material, "Modify Render Faces");

                switch (renderFaceValue)
                {
                    case RenderFace.Both:
                        {
                            material.SetFloat(cullName, 0);
                            break;
                        }
                    case RenderFace.Back:
                        {
                            material.SetFloat(cullName, 1);
                            break;
                        }
                    case RenderFace.Front:
                        {
                            material.SetFloat(cullName, 2);
                            break;
                        }
                }

                EditorUtility.SetDirty(material);
            }

            // Display blend mode options.
            bool blendModeChanged = false;
            if (surfaceTypeValue == SurfaceType.Transparent)
            {
                EditorGUI.BeginChangeCheck();
                {
                    blendModeValue = (BlendMode)EditorGUILayout.EnumPopup(new GUIContent(blendModeLabel, blendModeTooltip), blendModeValue);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(material, "Modify Blend Mode");

                    blendModeChanged = true;
                    material.SetFloat(blendModeName, (float)blendModeValue);
                    EditorUtility.SetDirty(material);
                }
            }

            bool alphaClipValue = material.GetFloat(alphaClip.name) > 0.5f;

            // Display alpha clip options.
            EditorGUI.BeginChangeCheck();
            {
                alphaClipValue = EditorGUILayout.Toggle(alphaClip.info, alphaClipValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(material, "Toggle Alpha Clip");
                surfaceTypeChanged = true;
            }

            material.SetFloat(alphaClip.name, alphaClipValue ? 1.0f : 0.0f);

            if (surfaceTypeChanged || blendModeChanged)
            {
                switch (surfaceTypeValue)
                {
                    case SurfaceType.Opaque:
                        {
                            material.SetOverrideTag("RenderType", "Opaque");
                            SetBlendMode(blendModeValue, surfaceTypeValue, material);
                            material.SetFloat(zWriteName, 1);
                            material.SetFloat(surfaceTypeName, 0);

                            alphaClipValue = material.GetFloat(alphaClip.name) >= 0.5f;
                            if (alphaClipValue)
                            {
                                material.EnableKeyword("_ALPHATEST_ON");
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                                material.SetOverrideTag("RenderType", "TransparentCutout");
                            }
                            else
                            {
                                material.DisableKeyword("_ALPHATEST_ON");
                                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                                material.SetOverrideTag("RenderType", "Opaque");
                            }


                            break;
                        }
                    case SurfaceType.Transparent:
                        {
                            alphaClipValue = material.GetFloat(alphaClip.name) >= 0.5f;
                            if (alphaClipValue)
                            {
                                material.EnableKeyword("_ALPHATEST_ON");
                            }
                            else
                            {
                                material.DisableKeyword("_ALPHATEST_ON");
                            }
                            material.SetOverrideTag("RenderType", "Transparent");
                            SetBlendMode(blendModeValue, surfaceTypeValue, material);
                            material.SetFloat(zWriteName, 0);
                            material.SetFloat(surfaceTypeName, 1);

                            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                            break;
                        }
                }

                EditorUtility.SetDirty(material);
            }

            alphaClipValue = material.GetFloat(alphaClip.name) >= 0.5f;
            if (alphaClipValue)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(alphaClipThreshold.prop, alphaClipThreshold.info);
                EditorGUI.indentLevel--;
            }

            if (receiveShadows.prop != null)
            {
                bool receiveShadowsValue = material.GetFloat(receiveShadows.name) > 0.5f;

                EditorGUI.BeginChangeCheck();
                {
                    receiveShadowsValue = EditorGUILayout.Toggle(receiveShadows.info, receiveShadowsValue);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(material, "Toggle Receive Shadows");

                    material.SetFloat(receiveShadows.name, receiveShadowsValue ? 1.0f : 0.0f);

                    if (receiveShadowsValue)
                    {
                        material.DisableKeyword("_RECEIVE_SHADOWS_OFF");
                    }
                    else
                    {
                        material.EnableKeyword("_RECEIVE_SHADOWS_OFF");
                    }

                    EditorUtility.SetDirty(material);
                }
            }
        }

        protected void DrawLitProperties(Material material)
        {
            EditorGUILayout.Space(3);

            EditorGUILayout.LabelField("Note: Base Texture tiling & offset settings are used for all Lit texture maps.", tinyLabelStyle);

            materialEditor.ShaderProperty(baseColor.prop, baseColor.info);
            materialEditor.ShaderProperty(baseTex.prop, baseTex.info);

            if (shouldRenderMetallic)
            {
                materialEditor.TexturePropertySingleLine(metallic.info, metallicTex.prop, metallic.prop);
            }

            if (shouldRenderSpecular)
            {
                materialEditor.TexturePropertySingleLine(specularColor.info, specularTex.prop, specularColor.prop);
            }

            if (smoothness.prop != null)
            {
                materialEditor.TexturePropertySingleLine(smoothness.info, smoothnessTex.prop, smoothness.prop);

                bool convertFromRoughValue = material.GetFloat(convertFromRoughness.name) > 0.5f;

                EditorGUI.BeginChangeCheck();
                {
                    convertFromRoughValue = EditorGUILayout.Toggle(convertFromRoughness.info, convertFromRoughValue);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(material, "Modify Convert From Rough");
                    material.SetFloat(convertFromRoughness.name, convertFromRoughValue ? 1.0f : 0.0f);
                    EditorUtility.SetDirty(material);
                }
            }

            if (normalMap.prop != null)
            {
                materialEditor.TexturePropertySingleLine(normalStrength.info, normalMap.prop, normalStrength.prop);
            }

            if (heightmapTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(heightmapStrength.info, heightmapTex.prop, heightmapStrength.prop);
            }

            if (occlusionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(occlusionStrength.info, occlusionTex.prop, occlusionStrength.prop);
            }

            if (emissionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(emissionColor.info, emissionTex.prop, emissionColor.prop);
            }
        }
    }
}
