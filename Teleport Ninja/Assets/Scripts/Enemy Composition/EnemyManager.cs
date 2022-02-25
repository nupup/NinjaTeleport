using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public enum EnemyState
    {
        None = 0,
        Patrol = 2,
        Alert = 5,
        Chase = 10,
        Confused = 15, //After lost track of target
        Wait = 20,
        Dead = 25
    }

    private float state_timer = 0f;
    private float wait_timer = 0f;

    [Header("State")]
    public EnemyState state = EnemyState.Patrol;


    [Header("State")]
    private EnemyGun enemyGun;


    void Awake()
    {
        enemyGun = GetComponent<EnemyGun>();
    }


    public void ChangeState(EnemyState state)
    {
        this.state = state;
        state_timer = 0f;
        wait_timer = 0f;
        waiting = false;

        switch (state)
        {
            case EnemyState.Alert:
                enemyGun.Shoot();
                break;
            case EnemyState.Patrol:
                enemyGun.StopShooting();
                break;
        }
    }



    void Update()
    {

        state_timer += Time.deltaTime;
        wait_timer += Time.deltaTime;
    }

    public EnemyState GetState()
    {
        return state;
    }
}
