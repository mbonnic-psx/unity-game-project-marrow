namespace ShaderToolboxPro.URP
{
    using UnityEngine;

    public class DemoHover : MonoBehaviour
    {
        [SerializeField] private Vector3 eulerRotation;
        [SerializeField] private Vector3 hoverDirection;
        [SerializeField] private float hoverSpeed;

        private Vector3 startPosition;

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.rotation = Quaternion.Euler(eulerRotation * Time.time);
            transform.position = startPosition + Mathf.Sin(Time.time * hoverSpeed * Mathf.PI) * hoverDirection;
        }
    }
}
