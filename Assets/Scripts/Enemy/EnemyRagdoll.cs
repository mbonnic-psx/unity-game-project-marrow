using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRagdoll : MonoBehaviour
{
    private Rigidbody[] rbBones;
    private Collider[] cBones;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private EnemyStateMachine esm;
    private Vector3[] bonePositions;
    private Quaternion[] boneRotations;

    void Awake()
    {
        esm = GetComponent<EnemyStateMachine>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        SetupRagdoll();
    }

    public void SetupRagdoll()
    {

        //This grabs every Collider and RB except for the main Collider and Root Rb
        rbBones = GetComponentsInChildren<Rigidbody>()
        .Where(rb => rb.gameObject != gameObject)
        .ToArray();

        cBones = GetComponentsInChildren<Collider>()
        .Where(c => c.gameObject != gameObject)
        .ToArray();

        //Cache original bone transforms
        bonePositions = new Vector3[rbBones.Length];
        boneRotations = new Quaternion[rbBones.Length];
        for (int i = 0; i < rbBones.Length; i++)
        {
            bonePositions[i] = rbBones[i].transform.localPosition;
            boneRotations[i] = rbBones[i].transform.localRotation;

        }

        foreach (var rb in rbBones)
        {
            rb.isKinematic = true;
        }

        foreach (var c in cBones)
        {
            c.enabled = false;
        }
    }

    public void EnableRagdoll()
    {
        animator.enabled = false;
        navMeshAgent.enabled = false;

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), true);

        foreach (var rb in rbBones)
        {
            rb.isKinematic = false;
        }

        foreach (var c in cBones)
        {
            c.enabled = true;
        }
    }

    public void DisableRagdoll()
    {
        Debug.Log("DisableRagdoll called, bones count: " + rbBones.Length);

        //Restore original bone transforms
        for (int i = 0; i < rbBones.Length; i++)
        {
            rbBones[i].transform.localPosition = bonePositions[i];
            rbBones[i].transform.localRotation = boneRotations[i];
        }

        animator.enabled = true;
        navMeshAgent.enabled = true;
        navMeshAgent.Warp(transform.position);

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"), false);

        foreach (var rb in rbBones)
        {
            rb.isKinematic = true;
        }

        foreach (var c in cBones)
        {
            c.enabled = false;
        }
    }
}
