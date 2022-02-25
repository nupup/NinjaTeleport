using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public Button KillButton;
    public DynamicJoystick mainJoystick;

    // Start is called before the first frame update
    void Start()
    {
        KillButton.onClick.AddListener(KillPressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void KillPressed()
    {
        GameManager.Instance.UpdateGameState(GameState.Killing);
    }

    public void AimingUI()
    {
        KillButton.gameObject.SetActive(true);
        mainJoystick.gameObject.SetActive(false);
    }

    public void KillingUI()
    {
        KillButton.gameObject.SetActive(false);
    }

    public void WalkingUI()
    {
        mainJoystick.gameObject.SetActive(true);
    }
}
