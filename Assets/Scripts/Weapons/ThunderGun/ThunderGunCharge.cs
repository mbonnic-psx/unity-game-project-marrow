using UnityEngine;

public class ThunderGunCharge : MonoBehaviour
{
    [Header("Charge Settings")]
    [SerializeField] private int killsToCharge = 10;
    [SerializeField] private int chargePerKill = 1;

    [Header("Feedback")]
    [SerializeField] private AudioClip chargeTickSound;    // plays each kill while charging
    [SerializeField] private AudioClip fullyChargedSound;  // plays when meter fills
    [SerializeField] private AudioClip dischargeSound;     // plays when blast fires

    int currentCharge;
    bool isCharged;
    AudioSource audioSource;

    public bool IsCharged => isCharged;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TrackKill()
    {
        if(isCharged)
        {
            return;
        }

        currentCharge = Mathf.Clamp(currentCharge + chargePerKill, 0, killsToCharge);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
