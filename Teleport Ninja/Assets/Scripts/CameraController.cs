using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera walkingCamera;
    [SerializeField]
    private CinemachineVirtualCamera aimingCamera;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwitchToWalk()
    {
        walkingCamera.Priority = 1;
        aimingCamera.Priority = 0;
    }
    public void SwitchToAim()
    {
        walkingCamera.Priority = 0;
        aimingCamera.Priority = 1;
    }
}
