using UnityEngine;

public class BillBoard : MonoBehaviour
{
    [SerializeField] private bool lookAtCamera = true;

    void Update()
    {
        if (lookAtCamera && Camera.main != null)
        {
            Vector3 targetPos = Camera.main.transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
        }
    }

    public void EnableLookAt()
    {
        lookAtCamera = true;
    }

    public void DisableLookAt()
    {
        lookAtCamera = false;
    }
}
