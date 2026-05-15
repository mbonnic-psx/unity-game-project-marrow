namespace ShaderToolboxPro.URP
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public class ExplosionPoint : MonoBehaviour
    {
        [SerializeField] private new Renderer renderer;

        private void Update()
        {
            if (renderer == null)
            {
                return;
            }

            var material = renderer.sharedMaterial;

            if (material != null)
            {

                material.SetVector("_ExplosionOriginPoint", transform.position);
            }
        }
    }
}
