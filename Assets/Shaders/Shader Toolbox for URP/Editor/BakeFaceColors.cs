namespace ShaderToolboxPro.URP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.VersionControl;
    using System.Reflection;

    public class BakeFaceColors : EditorWindow
    {
        private Mesh mesh;
        private Editor meshEditor;

        [MenuItem("Tools/Shader Toolbox/Bake Face Pos to Vertex Colors", priority = 10000)]
        public static void ShowWindow()
        {
            var window = GetWindowWithRect<BakeFaceColors>(new Rect(0, 0, 400, 300), false, "Bake Face Pos", true);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            mesh = EditorGUILayout.ObjectField(new GUIContent("Mesh"), mesh, typeof(Mesh), true) as Mesh;
            if(EditorGUI.EndChangeCheck())
            {
                if(meshEditor != null)
                {
                    DestroyImmediate(meshEditor);
                }
            }

            if (mesh != null)
            {
                if (mesh.isReadable)
                {
                    if (GUILayout.Button("Bake", GUILayout.Height(50)))
                    {
                        BakePositions();
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Mesh does not have read enabled.");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Please specify a mesh.");
            }

            GUILayout.FlexibleSpace();

            GUIStyle col = new GUIStyle();

            if (mesh != null)
            {
                if(meshEditor == null)
                {
                    meshEditor = Editor.CreateEditor(mesh);
                }

                meshEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 150), col);
            }
        }

        private void BakePositions()
        {
            Mesh meshToSave = Instantiate(mesh);

            var vertices = new List<Vector3>();
            meshToSave.GetVertices(vertices);

            var triangles = meshToSave.GetTriangles(0);

            var colors = new Color[vertices.Count];

            for(int t = 0; t < triangles.Length; t += 3)
            {
                Vector3 triCenter = (vertices[triangles[t]] + vertices[triangles[t + 1]] + vertices[triangles[t + 2]]) / 3.0f;
                colors[t] = colors[t + 1] = colors[t + 2] = new Color(triCenter.x, triCenter.y, triCenter.z, 0.0f);
            }

            meshToSave.SetColors(colors);

            // For some reason getting the current Project folder requires reflection.
            System.Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            string pathToCurrentFolder = obj.ToString();

            string name = AssetDatabase.GenerateUniqueAssetPath(pathToCurrentFolder + "/" + mesh.name + " (Baked).asset");
            AssetDatabase.CreateAsset(meshToSave, name);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
        }
    }
}
