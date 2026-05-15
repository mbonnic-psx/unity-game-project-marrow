namespace ShaderToolboxPro.URP
{
    using System;
    using UnityEditor;
    using UnityEditor.Rendering;
    using UnityEngine;

    public abstract class ToolboxShaderGUI : ShaderGUI
    {
        protected struct ToolboxProperty
        {
            public MaterialProperty prop;
            public readonly string name;
            public readonly GUIContent info;

            public ToolboxProperty(string name)
            {
                prop = null;
                this.name = name;
                info = null;
            }

            public ToolboxProperty(string name, GUIContent info)
            {
                prop = null;
                this.name = name;
                this.info = info;
            }

            public ToolboxProperty(string name, string prettyName, string description) :
                this(name, new GUIContent(prettyName, description))
            { }
        }

        protected enum SurfaceType
        {
            Opaque = 0,
            Transparent = 1
        }

        protected enum RenderFace
        {
            Front = 2,
            Back = 1,
            Both = 0
        }

        protected enum BlendMode
        {
            Alpha = 0,
            Premultiply = 1,
            Additive = 2,
            Multiply = 3
        }

        protected static GUIStyle _headerStyle;
        protected static GUIStyle headerStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle(GUI.skin.label)
                    {
                        wordWrap = true,
                        fontSize = 14,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleLeft
                    };
                }

                return _headerStyle;
            }
        }

        protected static GUIStyle _tinyLabelStyle;
        protected static GUIStyle tinyLabelStyle
        {
            get
            {
                if (_tinyLabelStyle == null)
                {
                    _tinyLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        wordWrap = true,
                        fontSize = 10,
                        fontStyle = FontStyle.Normal,
                        alignment = TextAnchor.MiddleLeft
                    };
                }

                return _tinyLabelStyle;
            }
        }

        protected static GUIStyle _descriptionStyle;
        protected static GUIStyle descriptionStyle
        {
            get
            {
                if (_descriptionStyle == null)
                {
                    _descriptionStyle = new GUIStyle(GUI.skin.label)
                    {
                        richText = true,
                        wordWrap = true,
                        fontStyle = FontStyle.Bold,
                        fontSize = 12
                    };
                }

                return _descriptionStyle;
            }
        }

        protected readonly MaterialHeaderScopeList materialScopeList = new MaterialHeaderScopeList(uint.MaxValue);
        protected MaterialEditor materialEditor;

        protected abstract string bannerTexturePath { get; }
        protected abstract string bannerFallbackText { get; }
        protected abstract string headerText { get; }
        protected Texture2D bannerTexture;

        protected bool firstTimeOpen = true;

        protected void SetBlendMode(BlendMode blendMode, SurfaceType surfaceType, Material material)
        {
            var srcBlendRGB = UnityEngine.Rendering.BlendMode.One;
            var dstBlendRGB = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            var srcBlendA = UnityEngine.Rendering.BlendMode.One;
            var dstBlendA = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;

            if (surfaceType == SurfaceType.Transparent)
            {
                switch (blendMode)
                {
                    case BlendMode.Alpha:
                        {
                            srcBlendRGB = UnityEngine.Rendering.BlendMode.SrcAlpha;
                            dstBlendRGB = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                            srcBlendA = UnityEngine.Rendering.BlendMode.One;
                            dstBlendA = dstBlendRGB;
                            break;
                        }
                    case BlendMode.Premultiply:
                        {
                            srcBlendRGB = UnityEngine.Rendering.BlendMode.One;
                            dstBlendRGB = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                            srcBlendA = srcBlendRGB;
                            dstBlendA = dstBlendRGB;
                            break;
                        }
                    case BlendMode.Additive:
                        {
                            srcBlendRGB = UnityEngine.Rendering.BlendMode.SrcAlpha;
                            dstBlendRGB = UnityEngine.Rendering.BlendMode.One;
                            srcBlendA = UnityEngine.Rendering.BlendMode.One;
                            dstBlendA = dstBlendRGB;
                            break;
                        }
                    case BlendMode.Multiply:
                        {
                            srcBlendRGB = UnityEngine.Rendering.BlendMode.DstColor;
                            dstBlendRGB = UnityEngine.Rendering.BlendMode.Zero;
                            srcBlendA = UnityEngine.Rendering.BlendMode.Zero;
                            dstBlendA = UnityEngine.Rendering.BlendMode.One;
                            break;
                        }
                }
            }
            else
            {
                srcBlendRGB = UnityEngine.Rendering.BlendMode.One;
                dstBlendRGB = UnityEngine.Rendering.BlendMode.Zero;
                srcBlendA = UnityEngine.Rendering.BlendMode.One;
                dstBlendA = UnityEngine.Rendering.BlendMode.Zero;
            }

            material.SetFloat("_SrcBlend", (float)srcBlendRGB);
            material.SetFloat("_DstBlend", (float)dstBlendRGB);
            material.SetFloat("_SrcBlendAlpha", (float)srcBlendA);
            material.SetFloat("_DstBlendAlpha", (float)dstBlendA);
        }

        protected void DrawDividerLine(Color color)
        {
            EditorGUILayout.Space(5);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = color;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        protected abstract void FindProperties(MaterialProperty[] props);

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            DrawBanner();

            if (materialEditor == null)
            {
                throw new ArgumentNullException("No MaterialEditor found (ToolboxShaderGUI).");
            }

            this.materialEditor = materialEditor;

            Material material = materialEditor.target as Material;
            materialScopeList.DrawHeaders(materialEditor, material);
        }

        protected void DrawBanner()
        {
            // Try and retrieve the banner texture if we don't have it yet.
            if (bannerTexture == null)
            {
                bannerTexture = Resources.Load<Texture2D>(bannerTexturePath);
            }

            if (bannerTexture != null)
            {
                float width = Mathf.Max(EditorGUIUtility.currentViewWidth, bannerTexture.height);
                float height = Mathf.Min(bannerTexture.height * (width / bannerTexture.width), bannerTexture.height);
                var bannerRect = new Rect(0, 0, width, height);
                GUI.DrawTexture(bannerRect, bannerTexture, ScaleMode.ScaleToFit, true);
                GUILayout.Space(height);
            }
            else
            {
                EditorGUILayout.LabelField(bannerFallbackText, headerStyle);
            }

            EditorGUILayout.LabelField(headerText, descriptionStyle);
            GUILayout.Space(5);
        }

        protected void DrawAdvancedSettings(Material material)
        {
            materialEditor.RenderQueueField();
            materialEditor.EnableInstancingField();
        }
    }
}
