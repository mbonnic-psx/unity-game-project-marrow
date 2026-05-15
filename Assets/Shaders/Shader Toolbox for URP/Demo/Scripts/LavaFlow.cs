namespace ShaderToolboxPro.URP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class LavaFlow : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        [SerializeField] private Vector2 flowSpeed = Vector2.one;

        private void Update()
        {
            if (renderer == null)
            {
                return;
            }

            var material = renderer.sharedMaterial;
            var offset = flowSpeed * Time.time;

            if (material != null)
            {
                material.SetTextureOffset("_BaseMap", offset); 
            }
        }
    }
}
