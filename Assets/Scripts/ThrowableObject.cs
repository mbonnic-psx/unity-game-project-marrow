using UnityEngine;
using DG.Tweening;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField] private float damageAmount = 100f;
    private bool wasThrown;
    private bool isStrongManThrown = false;

    public void SetThrown(bool value)
    {
        wasThrown = value;
    }

    public void SetStrongManThrown(bool active, float destroyDelay)
    {
        isStrongManThrown = active;

        if (active)
        {
            transform.DOScale(transform.localScale * 1.5f, 0.2f).SetEase(Ease.OutBack);
            Destroy(gameObject, destroyDelay);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (wasThrown == false)
        {
            return;
        }

        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            if (isStrongManThrown)
            {
                // Kill enemy on impact, but object lives on until destroyDelay
                enemyHealth.TakeDamage(Mathf.Infinity);
            }
            else
            {
                enemyHealth.TakeDamage(damageAmount);
                Destroy(gameObject);
            }
        }
        else
        {
            if (!isStrongManThrown)
            {
                wasThrown = false;
            }
        }
    }
}
