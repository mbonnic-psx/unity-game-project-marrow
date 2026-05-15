namespace ShaderToolboxPro.URP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class DissolvePlane : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;
        [SerializeField] private bool flipDirection;

        private void Update()
        {
            if (renderer == null)
            {
                return;
            }

            var material = renderer.sharedMaterial;

            if (material != null)
            {
                var direction = transform.forward;
                if (flipDirection)
                {
                    direction *= -1.0f;
                }

                material.SetVector("_CutoffPoint", transform.position);
                material.SetVector("_CutoffDirection", direction);
            }
        }
    }
}
