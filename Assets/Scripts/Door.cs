using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [SerializeField] private PlayerUI points;
    [SerializeField] private int doorCost;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<Collider> buyColliders;
    [SerializeField] private GameObject doorMesh;
    [SerializeField] private WaveManager waveManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (points == null)
        {
            points = FindAnyObjectByType<PlayerUI>();
        }

        doorMesh.SetActive(true);
    }

    public void BuyDoor()
    {
        if (points.KillCounter >= doorCost)
        {
            foreach (Transform spawn in spawnPoints)
            {
                waveManager.AddSpawnPoints(spawn);
            }

            foreach (Collider collider in buyColliders)
            {
                collider.enabled = false;
            }
            // Door Buy Animation
            doorMesh.SetActive(false);
        }
        else
        {
            return;
        }
    }
}
