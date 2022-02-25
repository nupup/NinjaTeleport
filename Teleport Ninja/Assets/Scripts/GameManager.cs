using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instances
    private CameraController cameraController;
    private AttackMoveController attackMoveController;
    private TimeManager timeManager;
    private ThirdPersonInput thirdPersonInput;
    private InGameUI inGameUI;

    //GM
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
        cameraController = GameObject.Find("Cinemachine").GetComponent<CameraController>();
        attackMoveController = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackMoveController>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        thirdPersonInput = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonInput>();
        inGameUI = GameObject.Find("InGame UI").GetComponent<InGameUI>();
    }

    private void Start()
    {
        UpdateGameState(GameState.Walking);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartScene();
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Walking:
                timeManager.RemoveSlowMotion();
                cameraController.SwitchToWalk();
                inGameUI.WalkingUI();
                break;
            case GameState.Aiming:
                cameraController.SwitchToAim();
                timeManager.DoSlowmotion();
                inGameUI.AimingUI();
                break;
            case GameState.Killing:
                inGameUI.KillingUI();
                timeManager.RemoveSlowMotion();
                attackMoveController.Kill();
                break;
            case GameState.QuickKilling:
                break;
            case GameState.Victory:
                StartCoroutine("NextLevel");
                break;
            case GameState.Lose:
                StartCoroutine("RestartScene");
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(2);
        try
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        catch (Exception e)
        {
            SceneManager.LoadScene(1);
        }
    }

    public bool AreEnemiesDead()
    {
        if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Victory);
            return true;
        }
        return false;
    }

}

public enum GameState
{
    Walking,
    Aiming,
    Killing,
    QuickKilling,
    Victory,
    Lose
}
