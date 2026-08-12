using Unity.VisualScripting;
using UnityEngine;

public class WinCondition : MonoBehaviour
{   
    [Header("Reference")]
    [SerializeField] private Power power;

    [Header("Win Condition")]
    [SerializeField] private PlayerUI points;
    [SerializeField] private int winCost;
    [SerializeField] private SceneManager sceneManager;

    [Header("Trains")]
    [SerializeField] private GameObject powerOffTrain;
    [SerializeField] private Collider powerOnTrain;

    void Start()
    {
        powerOffTrain.SetActive(true);
        powerOnTrain.enabled = false;
    }

    void Update()
    {
        if(power.powerFlag == true)
        {
            TrainSwitch();
        }
    }

    public void TrainSwitch()
    {
        if(power.powerFlag == true)
        {
            powerOffTrain.SetActive(false);
            powerOnTrain.enabled = true;  
        }
    }

    public void WinGame()
    {
        if(points.KillCounter >= winCost && power.powerFlag == true)
        {
            sceneManager.LoadGame(0);
        }
    }
}
