using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private float lastClickTime;
    public GameObject joyStick;


    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= DOUBLE_CLICK_TIME * (1/Time.timeScale) && GameManager.Instance.State == GameState.Walking)
            {
                GameManager.Instance.UpdateGameState(GameState.Aiming);
                
            }

            lastClickTime = Time.time;
        }
    }
}
