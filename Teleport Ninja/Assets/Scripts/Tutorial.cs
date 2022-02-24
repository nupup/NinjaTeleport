using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public GameObject doubleTapTip;
    public GameObject walkTutorial;
    private bool doubleTapTipShown;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
    }
    private void GameManagerOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Aiming:
                doubleTapTip.SetActive(false);
                break;
            case GameState.Lose:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !doubleTapTipShown)
        {
            doubleTapTipShown = true;
            walkTutorial.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {

                StartCoroutine(DoubleTapTip());
            }


        }
    }
    IEnumerator DoubleTapTip()
    {
        yield return new WaitForSeconds(1);
        doubleTapTip.SetActive(true);
    }
}
