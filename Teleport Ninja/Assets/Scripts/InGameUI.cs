using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Button killButton;
    public Button cancelKillButton;
    public DynamicJoystick mainJoystick;

    public GameObject killScreen;

    // Start is called before the first frame update
    void Start()
    {
        killButton.onClick.AddListener(KillPressed);
        cancelKillButton.onClick.AddListener(CancelKillPressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void KillPressed()
    {
        GameManager.Instance.UpdateGameState(GameState.Killing);
    }

    void CancelKillPressed()
    {
        GameManager.Instance.UpdateGameState(GameState.Walking);
    }

    public void AimingUI()
    {
        killScreen.gameObject.SetActive(true);
        mainJoystick.gameObject.SetActive(false);
    }

    public void KillingUI()
    {
        killScreen.gameObject.SetActive(false);
    }

    public void WalkingUI()
    {
        killScreen.gameObject.SetActive(false);
        mainJoystick.gameObject.SetActive(true);
    }
}
