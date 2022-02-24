using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    private const float DOUBLE_CLICK_TIME = 0.2f;
    private float lastClickTime;
    public GameObject joyStick;
    private AttackMoveController attackMoveController;


    private void Start()
    {
        attackMoveController = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackMoveController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            if (timeSinceLastClick <= DOUBLE_CLICK_TIME * (1/Time.timeScale) && GameManager.Instance.State == GameState.Walking)
            {
                attackMoveController.ProposeAim();
            }

            lastClickTime = Time.time;
        }
    }
}
