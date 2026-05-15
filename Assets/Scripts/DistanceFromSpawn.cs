using UnityEngine;

public class DistanceFromSpawn : MonoBehaviour
{
    private Vector3 spawnPoint;
    [SerializeField] private float maxDistance = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = transform.position;
        InvokeRepeating("OutOfBounds", 3f, 3f); //Check every 3 seconds
    }

    public void OutOfBounds()
    {
        float distanceFromSpawnPoint = Vector3.Distance(transform.position, spawnPoint);

        if (distanceFromSpawnPoint > maxDistance || transform.position.y < -50f)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        transform.position = spawnPoint;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero; //Stop momentum
    }
}
