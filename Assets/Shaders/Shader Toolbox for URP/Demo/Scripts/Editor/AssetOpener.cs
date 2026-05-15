using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShaderToolboxPro.URP
{
    [InitializeOnLoad]
    public class SceneNavigationEditor : Editor
    {
#if UNITY_EDITOR
        private const float buttonWidth = 200.0f;
        private const float buttonHeight = 40.0f;

        private static Object assetToOpen = null;
        private static List<OpenAssetButton> buttonsToCheck = new();

        protected static GUIStyle _buttonStyle;
        protected static GUIStyle ButtonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        richText = true,
                        wordWrap = true,
                        fontSize = 12
                    };
                }

                return _buttonStyle;
            }
        }

        static SceneNavigationEditor()
        {
            SceneView.duringSceneGui -= OnDuringSceneGUI;
            SceneView.duringSceneGui += OnDuringSceneGUI;

            SceneView.beforeSceneGui -= OnBeforeSceneGUI;
            SceneView.beforeSceneGui += OnBeforeSceneGUI;
        }

        private static void OnBeforeSceneGUI(SceneView sceneView)
        {
            if (assetToOpen != null)
            {
                var aboutToLoadAsset = assetToOpen;
                assetToOpen = null;

                AssetDatabase.OpenAsset(aboutToLoadAsset);
            }

            buttonsToCheck.Clear();

            var buttons = OpenAssetButton.Buttons;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i].assetToOpen == null)
                {
                    continue;
                }

                var buttonPos = buttons[i].transform.position;
                var cameraPos = sceneView.camera.transform.position;

                if (Vector3.Distance(buttonPos, cameraPos) < 15.0f)
                {
                    if (Vector3.Dot(sceneView.camera.transform.forward, buttonPos - cameraPos) > 0.0f)
                    {
                        buttonsToCheck.Add(buttons[i]);
                    }
                }
            }
        }

        private static void OnDuringSceneGUI(SceneView sceneView)
        {
            Handles.BeginGUI();

            var buttons = OpenAssetButton.Buttons;

            for (int i = 0; i < buttonsToCheck.Count; i++)
            {
                var btn = buttonsToCheck[i];
                var buttonPos = btn.transform.position;

                var position = HandleUtility.WorldToGUIPoint(buttonPos);
                var rect = new Rect(position.x - buttonWidth / 2.0f, position.y - buttonHeight / 2.0f, buttonWidth, buttonHeight);

                if (GUI.Button(rect, new GUIContent($"Open <b>{btn.assetName}</b>", $"Open the {btn.assetName} asset."), ButtonStyle))
                {
                    assetToOpen = btn.assetToOpen;
                }
            }

            Handles.EndGUI();
        }
#endif
    }
}
