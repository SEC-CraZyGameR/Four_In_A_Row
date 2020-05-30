using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicTacToeUI : MonoBehaviour
{
    public static TicTacToeUI Instance;
    public Button BtnBackToMain;
    public GameObject panelMenu;
    public Button btnPlay;

    [Space(2)]
    [Header("Difficulty Panel")]
    public GameObject panelDifficulty;
    public Button btnEasy;
    public Button btnMedium;
    public Button btnHard;
    private void Start()
    {
        Instance = this;
        BtnBackToMain.onClick.AddListener(() => BackToMainCallBack());
        btnPlay.onClick.AddListener(() => PlayCallBack());
        btnEasy.onClick.AddListener(() => EasyCallBack());
        btnMedium.onClick.AddListener(() => MediumCallBack());
        btnHard.onClick.AddListener(() => HardCallBack());
    }

    private void PlayCallBack()
    {
        GameManager.Instance.StartGame();
        Constant.SetActiveFalse(panelMenu);
        panelDifficulty.SetActive(true);
    }
    private void BackToMainCallBack()
    {
        SceneManager.LoadScene(0);
    }
    public void PlayAgainCallBack()
    {
        UiManager.Instance.txtWhoseTurn.gameObject.SetActive(true);
        UiManager.Instance.txtWinner.gameObject.SetActive(false);
        GameManager.Instance.isGameStart = true;
        GameManager.Instance.isGameOver = false;
        TicTacToe.Instance.ResetGame();
    }
    private void HardCallBack()
    {
        SetDiffiCulty(9);
    }

    private void MediumCallBack()
    {
        SetDiffiCulty(5);
    }

    private void EasyCallBack()
    {
        SetDiffiCulty(1);
    }

    private void SetDiffiCulty(int difficulty)
    {
        UiManager.Instance.DisplayWhoseTurn(Player.Human);
        Constant.Difficulty = difficulty;
        panelDifficulty.gameObject.SetActive(false);
    }
}
