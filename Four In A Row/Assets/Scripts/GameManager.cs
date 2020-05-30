using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool isGameStart = false;
    public bool isGameOver = false;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        isGameStart = true;
    }

    public void GameOver()
    {
        isGameOver = true;
        //UiManager.Instance.GameOverMenu();
    }
}
