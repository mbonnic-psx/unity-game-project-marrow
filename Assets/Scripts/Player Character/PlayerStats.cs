using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    [SerializeField] private float maxHealth = 99f;
    [SerializeField] private float currentHealth;
    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private Camera camera;

    [Header("Death Sequence")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 4f;
    [SerializeField] private int sceneIndex;

    private Rigidbody cameraRb;

    #region Getters
    public float PlayerHealth => maxHealth;
    public float CurrentHealth => currentHealth;
    #endregion

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Start()
    {
        cameraRb = camera.GetComponent<Rigidbody>();

        if (cameraRb != null)
        {
            cameraRb.isKinematic = true;
            cameraRb.useGravity = false;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);
        if (currentHealth <= 0)
        {
            PlayerDied();
            return;
        }

        StopAllCoroutines();
        HealPlayer();
    }

    public void HealPlayer()
    {
        StartCoroutine(Heal());
        IEnumerator Heal()
        {
            yield return new WaitForSeconds(5f);
            currentHealth = maxHealth;
        }

    }

    public void PlayerDied()
    {
        Debug.Log("The Player Died");
        PlayerMovement.enabled = false;
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        camera.transform.SetParent(null);

        if (cameraRb != null)
        {
            cameraRb.isKinematic = false;
            cameraRb.useGravity = true;
        }

        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void SetPlayerHealth(float h)
    {
        maxHealth = h;
        currentHealth = h;
    }
}
