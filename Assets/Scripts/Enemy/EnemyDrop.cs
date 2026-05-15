using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private GameObject[] dropPrefebs;
    [SerializeField] private float dropChance = 0.75f;
    [SerializeField] private float impulseForce = 5f;
    [SerializeField] private int maxDrops = 3;
    private Rigidbody rb;

    public void DropItems()
    {
        for (int i = 0; i < maxDrops; i++)
        {
            if (Random.value <= dropChance)
            {
                //Debug.Log("Dropping Items");
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f)).normalized;
                GameObject droppedItems = Instantiate(dropPrefebs[Random.Range(0, dropPrefebs.Length)], transform.position + Vector3.up * 1f, Quaternion.identity);
                rb = droppedItems.GetComponent<Rigidbody>();
                rb.isKinematic = false;
                rb.AddForce(randomDirection * impulseForce, ForceMode.Impulse);
            }
            else
            {
                return;
            }
        }

    }

}
