using UnityEngine;

public class Pickup : MonoBehaviour
{
    public void PickedupItem()
    {
        Debug.Log("Pickedup Item");
        Destroy(gameObject);
    }
}
