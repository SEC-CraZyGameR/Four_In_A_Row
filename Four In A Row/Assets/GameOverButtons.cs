using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverButtons : MonoBehaviour
{
    public static GameOverButtons Instance;

    public Button btnPlayAgain;
    public Button btnHome;
    int _tag;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        btnPlayAgain.onClick.AddListener(() => PlayAgainCallback());
        btnHome.onClick.AddListener(() => HomeCallback());
    }

    private void HomeCallback()
    {
        UiManager.Instance.InvokeAction(0);
        Destroy(this.gameObject);
    }

    private void PlayAgainCallback()
    {
        UiManager.Instance.InvokeAction(1);
        Destroy(this.gameObject);
    }

    public void SetTag(int tag)
    {
        _tag = tag;
    }
}
