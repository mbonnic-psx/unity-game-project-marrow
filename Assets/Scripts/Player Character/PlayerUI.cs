using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killCounterText;
    [SerializeField] private TextMeshProUGUI roundCounterText;
    [SerializeField] private TextMeshProUGUI enemyCounterText;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private int killCounter = 0;

    public int KillCounter => killCounter;

    public void Start()
    {
        if(waveManager == null)
        {
            waveManager = FindAnyObjectByType<WaveManager>();
        }
    }

    public void Update()
    {
        roundCounterText.text = waveManager.CurrentWave.ToString();
        enemyCounterText.text = "Enemies Remaining: " + waveManager.EnemiesAlive.ToString();
    }

    public string Counter()
    {
        killCounter++;
        killCounterText.text = killCounter.ToString();
        return killCounter.ToString();
    }
}
