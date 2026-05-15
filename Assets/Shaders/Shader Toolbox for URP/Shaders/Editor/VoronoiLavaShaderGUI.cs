namespace ShaderToolboxPro.URP
{
    using UnityEditor;
    using UnityEditor.Rendering.Universal.ShaderGUI;
    using UnityEngine;

    public class VoronoiLavaShaderGUI : ToolboxShaderGUI
    {
        private ToolboxProperty baseColor = new("_BaseColor", "Base Color", "Albedo color applied to entire mesh." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty baseTex = new("_BaseMap", "Base Texture", "Albedo texture applied to entire mesh." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty workflowMode = new("_WorkflowMode", "Workflow Mode", "");
        private ToolboxProperty metallic = new("_Metallic", "Metallic", "How metallic the surface should be." +
            "\n1 represents a metal, and 0 represents a non-metal." +
            "\nVery few objects in the real world use values around 0.5." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty metallicTex = new("_MetallicGlossMap");
        private ToolboxProperty specularColor = new("_SpecColor", "Specular Color", "What color should be used for specular highlights." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty specularTex = new("_SpecGlossMap");
        private ToolboxProperty smoothness = new("_Smoothness", "Smoothness", "How smooth the surface of the object should be." +
            "\n1 reprsents a highly polished surface. 0 represents a very rough or matter surface." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty smoothnessTex = new("_SmoothnessMap");
        private ToolboxProperty convertFromRoughness = new("_ConvertFromRoughness", "Convert From Roughness", "Does this material use a roughness texture instead of smoothness?" +
            "\n\nApplies to layer 1.");
        private ToolboxProperty normalStrength = new("_BumpScale", "Normal Map", "Normal map modifies the surface normals for finer lighting detail." +
            "\n1 represents 'standard' strength." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty normalMap = new("_BumpMap");
        private ToolboxProperty heightmapStrength = new("_Parallax", "Heightmap", "A heightmap can be used to 'fake' raised and lower sections on the surface." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty heightmapTex = new("_ParallaxMap");
        private ToolboxProperty occlusionStrength = new("_OcclusionStrength", "Ambient Occlusion", "Amount of ambient occlusion falling on the surface." +
            "\n1 represents a fully lit part of the surface, while 0 means a fully shadowed area." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty occlusionTex = new("_OcclusionMap");
        private ToolboxProperty emissionColor = new("_EmissionColor", "Emission Color", "The amount of emissive light to use on the surface." +
            "\nWhereas Base Color is influenced by scene lighting, emissive color is visible regardless of whether the object is in shadow." +
            "\n\nApplies to layer 1.");
        private ToolboxProperty emissionTex = new("_EmissionMap");

        private ToolboxProperty baseColor2 = new("_BaseColor_2", "Base Color", "Albedo color applied to entire mesh." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty baseTex2 = new("_BaseMap_2", "Base Texture", "Albedo texture applied to entire mesh." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty metallic2 = new("_Metallic_2", "Metallic", "How metallic the surface should be." +
            "\n1 represents a metal, and 0 represents a non-metal." +
            "\nVery few objects in the real world use values around 0.5." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty metallicTex2 = new("_MetallicGlossMap_2");
        private ToolboxProperty specularColor2 = new("_SpecColor_2", "Specular Color", "What color should be used for specular highlights." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty specularTex2 = new("_SpecGlossMap_2");
        private ToolboxProperty smoothness2 = new("_Smoothness_2", "Smoothness", "How smooth the surface of the object should be." +
            "\n1 reprsents a highly polished surface. 0 represents a very rough or matter surface." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty smoothnessTex2 = new("_SmoothnessMap_2");
        private ToolboxProperty convertFromRoughness2 = new("_ConvertFromRoughness_2", "Convert From Roughness", "Does this material use a roughness texture instead of smoothness?" +
            "\n\nApplies to layer 2.");
        private ToolboxProperty normalStrength2 = new("_BumpScale_2", "Normal Map", "Normal map modifies the surface normals for finer lighting detail." +
            "\n1 represents 'standard' strength." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty normalMap2 = new("_BumpMap_2");
        private ToolboxProperty heightmapStrength2 = new("_Parallax", "Heightmap", "A heightmap can be used to 'fake' raised and lower sections on the surface." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty heightmapTex2 = new("_ParallaxMap_2");
        private ToolboxProperty occlusionStrength2 = new("_OcclusionStrength_2", "Ambient Occlusion", "Amount of ambient occlusion falling on the surface." +
            "\n1 represents a fully lit part of the surface, while 0 means a fully shadowed area." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty occlusionTex2 = new("_OcclusionMap_2");
        private ToolboxProperty emissionColor2 = new("_EmissionColor_2", "Emission Color", "The amount of emissive light to use on the surface." +
            "\nWhereas Base Color is influenced by scene lighting, emissive color is visible regardless of whether the object is in shadow." +
            "\n\nApplies to layer 2.");
        private ToolboxProperty emissionTex2 = new("_EmissionMap_2");

        private ToolboxProperty voronoiDensity = new("_VoronoiDensity", "Voronoi Density", "How closely the Voronoi cells are packed together across the object surface.");
        private ToolboxProperty voronoiAngleOffset = new("_VoronoiAngleOffset", "Voronoi Angle Offset", "Affects the random offsets used by the Voronoi algorithm. Changing this value changes the positioning of cells.");
        private ToolboxProperty voronoiThickness = new("_VoronoiThickness", "Voronoi Thickness", "How thick the edges between Voronoi cells are (the edge pattern forms a mask to choose between texture layers).");
        private ToolboxProperty voronoiFalloff = new("_VoronoiFalloff", "Voronoi Falloff", "Smoothness of the transition between Voronoi edges and the cell centers.");
        
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

        protected override string bannerTexturePath { get { return "VoronoiLavaBanner"; } }
        protected override string bannerFallbackText { get { return "Voronoi Lava"; } }
        protected override string headerText { get { return "An effect which picks between two PBR layers based on a Voronoi edge pattern."; } }

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

            baseColor2.prop = FindProperty(baseColor2.name, props, true);
            baseTex2.prop = FindProperty(baseTex2.name, props, true);
            metallic2.prop = FindProperty(metallic2.name, props, true);
            metallicTex2.prop = FindProperty(metallicTex2.name, props, true);
            specularColor2.prop = FindProperty(specularColor2.name, props, true);
            specularTex2.prop = FindProperty(specularTex2.name, props, true);
            smoothness2.prop = FindProperty(smoothness2.name, props, true);
            smoothnessTex2.prop = FindProperty(smoothnessTex2.name, props, true);
            convertFromRoughness2.prop = FindProperty(convertFromRoughness2.name, props, true);
            normalMap2.prop = FindProperty(normalMap2.name, props, true);
            normalStrength2.prop = FindProperty(normalStrength2.name, props, true);
            heightmapStrength2.prop = FindProperty(heightmapStrength2.name, props, true);
            heightmapTex2.prop = FindProperty(heightmapTex2.name, props, true);
            occlusionStrength2.prop = FindProperty(occlusionStrength2.name, props, true);
            occlusionTex2.prop = FindProperty(occlusionTex2.name, props, true);
            emissionColor2.prop = FindProperty(emissionColor2.name, props, true);
            emissionTex2.prop = FindProperty(emissionTex2.name, props, true);

            voronoiDensity.prop = FindProperty(voronoiDensity.name, props, true);
            voronoiAngleOffset.prop = FindProperty(voronoiAngleOffset.name, props, true);
            voronoiThickness.prop = FindProperty(voronoiThickness.name, props, true);
            voronoiFalloff.prop = FindProperty(voronoiFalloff.name, props, true);

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
                materialScopeList.RegisterHeaderScope(new GUIContent("Voronoi Properties"), 1u << 1, DrawVoronoiProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("First Layer Properties"), 1u << 2, DrawFirstLayerProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Second Layer Properties"), 1u << 3, DrawSecondLayerProperties);
                materialScopeList.RegisterHeaderScope(new GUIContent("Advanced Settings"), 1u << 4, DrawAdvancedSettings);
                firstTimeOpen = false;
            }

            base.OnGUI(materialEditor, properties);
        }

        private void DrawSurfaceOptions(Material material)
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

            if(GUILayout.Button(new GUIContent("Swap Texture Layers", "Swap all properties in Layer 1 with Layer 2.")))
            {
                SwapLayers(material);
            }
        }

        private void DrawVoronoiProperties(Material material)
        {
            materialEditor.ShaderProperty(voronoiDensity.prop, voronoiDensity.info);
            materialEditor.ShaderProperty(voronoiAngleOffset.prop, voronoiAngleOffset.info);
            materialEditor.ShaderProperty(voronoiThickness.prop, voronoiThickness.info);
            materialEditor.ShaderProperty(voronoiFalloff.prop, voronoiFalloff.info);
        }

        private void DrawFirstLayerProperties(Material material)
        {
            EditorGUILayout.Space(3);

            EditorGUILayout.LabelField("Note: Base Texture tiling & offset settings are used for all Layer 1 texture maps.", tinyLabelStyle);

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
                    convertFromRoughValue = EditorGUILayout.Toggle(convertFromRoughness.name, convertFromRoughValue);
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
                materialEditor.TexturePropertySingleLine(heightmapStrength.info,heightmapTex.prop, heightmapStrength.prop);
            }

            if (occlusionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(occlusionStrength.info, occlusionTex.prop, occlusionStrength.prop);
            }

            if (emissionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(emissionColor.info,emissionTex.prop, emissionColor.prop);
            }
        }

        private void DrawSecondLayerProperties(Material material)
        {
            EditorGUILayout.Space(3);

            EditorGUILayout.LabelField("Note: Base Texture tiling & offset settings are used for all Layer 2 texture maps.", tinyLabelStyle);

            materialEditor.ShaderProperty(baseColor2.prop, baseColor2.info);
            materialEditor.ShaderProperty(baseTex2.prop, baseTex2.info);

            if (shouldRenderMetallic)
            {
                materialEditor.TexturePropertySingleLine(metallic2.info, metallicTex.prop, metallic2.prop);
            }

            if (shouldRenderSpecular)
            {
                materialEditor.TexturePropertySingleLine(specularColor2.info, specularTex2.prop, specularColor2.prop);
            }

            if (smoothness.prop != null)
            {
                materialEditor.TexturePropertySingleLine(smoothness2.info, smoothnessTex2.prop, smoothness2.prop);

                bool convertFromRoughValue = material.GetFloat(convertFromRoughness2.name) > 0.5f;

                EditorGUI.BeginChangeCheck();
                {
                    convertFromRoughValue = EditorGUILayout.Toggle(convertFromRoughness2.info, convertFromRoughValue);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(material, "Modify Convert From Rough");
                    material.SetFloat(convertFromRoughness2.name, convertFromRoughValue ? 1.0f : 0.0f);
                    EditorUtility.SetDirty(material);
                }
            }

            if (normalMap.prop != null)
            {
                materialEditor.TexturePropertySingleLine(normalStrength2.info, normalMap2.prop, normalStrength2.prop);
            }

            if (heightmapTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(heightmapStrength2.info, heightmapTex2.prop, heightmapStrength2.prop);
            }

            if (occlusionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(occlusionStrength2.info, occlusionTex2.prop, occlusionStrength2.prop);
            }

            if (emissionTex.prop != null)
            {
                materialEditor.TexturePropertySingleLine(emissionColor2.info, emissionTex2.prop, emissionColor2.prop);
            }
        }

        public void SwapLayers(Material material)
        {
            var baseColorValue = material.GetColor(baseColor.name);
            var baseTexValue = material.GetTexture(baseTex.name);
            var metallicValue = material.GetFloat(metallic.name);
            var metallicTexValue = material.GetTexture(metallicTex.name);
            var specularColorValue = material.GetColor(specularColor.name);
            var specularTexValue = material.GetTexture(specularTex.name);
            var smoothnessValue = material.GetFloat(smoothness.name);
            var smoothnessTexValue = material.GetTexture(smoothnessTex.name);
            var convertFromRoughnessValue = material.GetFloat(convertFromRoughness.name);
            var normalTexValue = material.GetTexture(normalMap.name);
            var normalStrengthValue = material.GetFloat(normalStrength.name);
            var heightmapStrengthValue = material.GetFloat(heightmapStrength.name);
            var heightmapTexValue = material.GetTexture(heightmapTex.name);
            var occlusionStrengthValue = material.GetFloat(occlusionStrength.name);
            var occlusionTexValue = material.GetTexture(occlusionTex.name);
            var emissionColorValue = material.GetColor(emissionColor.name);
            var emissionTexValue = material.GetTexture(emissionTex.name);

            var baseColor2Value = material.GetColor(baseColor2.name);
            var baseTex2Value = material.GetTexture(baseTex2.name);
            var metallic2Value = material.GetFloat(metallic2.name);
            var metallicTex2Value = material.GetTexture(metallicTex2.name);
            var specularColor2Value = material.GetColor(specularColor2.name);
            var specularTex2Value = material.GetTexture(specularTex2.name);
            var smoothness2Value = material.GetFloat(smoothness2.name);
            var smoothnessTex2Value = material.GetTexture(smoothnessTex2.name);
            var convertFromRoughness2Value = material.GetFloat(convertFromRoughness2.name);
            var normalTex2Value = material.GetTexture(normalMap2.name);
            var normalStrength2Value = material.GetFloat(normalStrength2.name);
            var heightmapStrength2Value = material.GetFloat(heightmapStrength2.name);
            var heightmapTex2Value = material.GetTexture(heightmapTex2.name);
            var occlusionStrength2Value = material.GetFloat(occlusionStrength2.name);
            var occlusionTex2Value = material.GetTexture(occlusionTex2.name);
            var emissionColor2Value = material.GetColor(emissionColor2.name);
            var emissionTex2Value = material.GetTexture(emissionTex2.name);

            Undo.RecordObject(material, "Swap Texture Layers");

            material.SetColor(baseColor.name, baseColor2Value);
            material.SetTexture(baseTex.name, baseTex2Value);
            material.SetFloat(metallic.name, metallic2Value);
            material.SetTexture(metallicTex.name, metallicTex2Value);
            material.SetColor(specularColor.name, specularColor2Value);
            material.SetTexture(specularTex.name, specularTex2Value);
            material.SetFloat(smoothness.name, smoothness2Value);
            material.SetTexture(smoothnessTex.name, smoothnessTex2Value);
            material.SetFloat(convertFromRoughness.name, convertFromRoughness2Value);
            material.SetTexture(normalMap.name, normalTex2Value);
            material.SetFloat(normalStrength.name, normalStrength2Value);
            material.SetFloat(heightmapStrength.name, heightmapStrength2Value);
            material.SetTexture(heightmapTex.name, heightmapTex2Value);
            material.SetFloat(occlusionStrength.name, occlusionStrength2Value);
            material.SetTexture(occlusionTex.name, occlusionTex2Value);
            material.SetColor(emissionColor.name, emissionColor2Value);
            material.SetTexture(emissionTex.name, emissionTex2Value);

            material.SetColor(baseColor2.name, baseColorValue);
            material.SetTexture(baseTex2.name, baseTexValue);
            material.SetFloat(metallic2.name, metallicValue);
            material.SetTexture(metallicTex2.name, metallicTexValue);
            material.SetColor(specularColor2.name, specularColorValue);
            material.SetTexture(specularTex2.name, specularTexValue);
            material.SetFloat(smoothness2.name, smoothnessValue);
            material.SetTexture(smoothnessTex2.name, smoothnessTexValue);
            material.SetFloat(convertFromRoughness2.name, convertFromRoughnessValue);
            material.SetTexture(normalMap2.name, normalTexValue);
            material.SetFloat(normalStrength2.name, normalStrengthValue);
            material.SetFloat(heightmapStrength2.name, heightmapStrengthValue);
            material.SetTexture(heightmapTex2.name, heightmapTexValue);
            material.SetFloat(occlusionStrength2.name, occlusionStrengthValue);
            material.SetTexture(occlusionTex2.name, occlusionTexValue);
            material.SetColor(emissionColor2.name, emissionColorValue);
            material.SetTexture(emissionTex2.name, emissionTexValue);

            EditorUtility.SetDirty(material);
        }
    }
}
