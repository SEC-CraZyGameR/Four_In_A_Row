using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FourInARowUI : MonoBehaviour
{
    public static FourInARowUI Instance;
    [Header("Main Menu Components")]
    public GameObject panelMenu;
    public Button btnPlay;


    [Space(2)]
    [Header("Difficulty Panel")]
    public GameObject panelDifficulty;
    public Button btnEasy;
    public Button btnMedium;
    public Button btnHard;


    public Button btnBackToMain;



    private void Start()
    {
        Instance = this;
        panelMenu.SetActive(true);
        btnPlay.onClick.AddListener(() => PlayCallBack());

        btnEasy.onClick.AddListener(() => EasyCallBack());
        btnMedium.onClick.AddListener(() => MediumCallBack());
        btnHard.onClick.AddListener(() => HardCallBack());
        btnBackToMain.onClick.AddListener(() => BackToMainMenuCallBack());
    }

    private void BackToMainMenuCallBack()
    {
        HomeCallBack();
    }

    private void HardCallBack()
    {
        SetDiffiCulty(5);
    }

    private void MediumCallBack()
    {
        SetDiffiCulty(3);
    }

    private void EasyCallBack()
    {
        SetDiffiCulty(2);
    }

    private void SetDiffiCulty(int difficulty)
    {
        UiManager.Instance.DisplayWhoseTurn(Player.Human);
        Constant.Difficulty = difficulty;
        panelDifficulty.gameObject.SetActive(false);
    }

    private void HomeCallBack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayAgainCallBack()
    {
        UiManager.Instance.txtWhoseTurn.gameObject.SetActive(true);
        UiManager.Instance.txtWinner.gameObject.SetActive(false);
        GameManager.Instance.isGameStart = true;
        GameManager.Instance.isGameOver = false;
        FourInARow.Instance.ResetGame();
    }

    private void PlayCallBack()
    {
        GameManager.Instance.StartGame();
        Constant.SetActiveFalse(panelMenu);
        panelDifficulty.SetActive(true);
    }

}
