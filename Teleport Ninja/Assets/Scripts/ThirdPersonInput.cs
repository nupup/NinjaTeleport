using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;
using Cinemachine;

public class ThirdPersonInput : MonoBehaviour
{
    public DynamicJoystick mainJoystick;
    protected ThirdPersonUserControl control;
    [SerializeField]
    private CinemachineVirtualCamera freelookCamera;

    //Rotate when aiming
    GameObject player;
    AttackMoveController attackMoveController;

    private Touch touch;
    private Vector2 touchPosition;
    private Quaternion rotationY;
    private float rotationX;
    [SerializeField]
    private float rotationSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        control = GetComponent<ThirdPersonUserControl>();
        player = GameObject.FindGameObjectWithTag("Player");
        attackMoveController = player.GetComponent<AttackMoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.State == GameState.Walking)
        {
            control.HInput = mainJoystick.Direction.x;
            control.VInput = mainJoystick.Direction.y;
        }

        //Rotate if aiming
        if (GameManager.Instance.State == GameState.Aiming)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);


                if (touch.phase == TouchPhase.Moved)
                {
                    Debug.Log("touch without it");
                    rotationY = Quaternion.Euler(
                        0f,
                        touch.deltaPosition.x * rotationSpeed * Time.deltaTime,
                        0f);

                    rotationX = freelookCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY += (touch.deltaPosition.y * rotationSpeed/50 * Time.deltaTime);
                    rotationX = Mathf.Clamp(rotationX, 0.5555728f , 1.05f);

                    //player rot (y axis)
                    player.transform.rotation = rotationY * player.transform.rotation;
                    //camera rot (x axis)
                    freelookCamera.GetCinemachineComponent<CinemachineComposer>().m_ScreenY = rotationX;
                }
            }
        }
    }
}
