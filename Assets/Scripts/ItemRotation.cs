using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50, 0);
    private bool shouldRotate = false;

    void Start()
    {
        shouldRotate = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRotate)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }

    }

    public void ActivateRotation()
    {
        shouldRotate = true;
    }
}
