using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TicTacToe : MonoBehaviour
{
    public static TicTacToe Instance;

    public List<Button> Cells;
    public Button[,] GameBoard = new Button[3, 3];
    public int[,] Board = new int[3, 3];

    public Sprite O;
    public Sprite X;

    int length = 3;
    int human = 1;
    int ai = 2;
    int empty = 0;
    int player = -1;

    int previousWinState = 1;

    private void Start()
    {
        Instance = this;

        UiManager.Instance.action += GameOverBtnsCallBack;

        int cellindex = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameBoard[i, j] = Cells[cellindex++];
                Board[i, j] = empty;
            }
        }

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var x = i;
                var y = j;
                GameBoard[i, j].onClick.AddListener(() => ButtonCallBack(x, y));
            }
        }

        SetPlayer(human);
        //StartCoroutine(AiTurn());
    }

    public void ResetGame()
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                GameBoard[i, j].image.sprite = default;
                Board[i, j] = 0;
            }
        }
        if (previousWinState == 0)
        {
            int player = UnityEngine.Random.Range(1, 3);
            SetPlayer(player);
            if (player == 1)
            {
                UiManager.Instance.DisplayWhoseTurn(Player.Human);
            }
            else
            {
                StartCoroutine(AiTurn());
                UiManager.Instance.DisplayWhoseTurn(Player.Ai);
            }
        }
        else
        {
            if (previousWinState == 1)
            {
                StartCoroutine(AiTurn());
                SetPlayer(2);
                UiManager.Instance.DisplayWhoseTurn(Player.Ai);
            }
            else
            {
                SetPlayer(1);
                UiManager.Instance.DisplayWhoseTurn(Player.Human);
            }
        }
    }
    private void GameOverBtnsCallBack(int tag, int index)
    {
        if (tag == 1)
        {
            if (index == 1)
                TicTacToeUI.Instance.PlayAgainCallBack();
            else if (index == 0)
                SceneManager.LoadScene(0);
        }
    }
    private void OnDestroy()
    {
        UiManager.Instance.action -= GameOverBtnsCallBack;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void SetPlayer(int _player)
    {
        player = _player;
    }

    private void ButtonCallBack(int i, int j)
    {
        if (!GameManager.Instance.isGameStart || GameManager.Instance.isGameOver)
            return;
        if (player == human)
        {
            if (Board[i, j] == empty)
            {
                Board[i, j] = human;
                GameBoard[i, j].image.sprite = O;
                if (EvaluateScore(Board) == -10)
                {
                    //GameOver
                    previousWinState = 1;
                    GameManager.Instance.GameOver();
                    UiManager.Instance.DisPlayWinner(Player.Human);
                    UiManager.Instance.GameOverMenu(1);
                    return;
                }
                if (isMoveLeft(Board))
                {
                    SetPlayer(ai);
                    UiManager.Instance.DisplayWhoseTurn(Player.Ai);
                    StartCoroutine(AiTurn());
                }
                else
                {
                    //Draw
                    previousWinState = 0;
                    UiManager.Instance.GameOverMenu(1);
                    UiManager.Instance.DisPlayWinner(0);
                }
            }
        }
    }

    private IEnumerator AiTurn()
    {
        yield return new WaitForSeconds(0.5f);
        var move = FindBestMove(Board);
        Board[move.Item1, move.Item2] = ai;
        GameBoard[move.Item1, move.Item2].image.sprite = X;

        if (EvaluateScore(Board) == +10)
        {
            previousWinState = 2;
            UiManager.Instance.DisPlayWinner(Player.Ai);
            UiManager.Instance.GameOverMenu(1);
            GameManager.Instance.GameOver();
            yield break;
        }
        if (isMoveLeft(Board))
        {
            SetPlayer(human);
            UiManager.Instance.DisplayWhoseTurn(Player.Human);
        }
        else
        {
            previousWinState = 0;
            UiManager.Instance.GameOverMenu(1);
            UiManager.Instance.DisPlayWinner(0);
        }
    }

    public bool isMoveLeft(int[,] Board)
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                if (Board[i, j] == empty)
                    return true;
            }
        }
        return false;
    }
    public int EvaluateScore(int[,] Board)
    {
        for (int row = 0; row < length; row++)
        {
            if (Board[row, 0] == Board[row, 1] && Board[row, 1] == Board[row, 2])
            {
                if (Board[row, 0] == human)
                    return -10;
                else if (Board[row, 0] == ai)
                    return +10;
            }
        }
        for (int col = 0; col < length; col++)
        {
            if (Board[0, col] == Board[1, col] && Board[1, col] == Board[2, col])
            {
                if (Board[0, col] == human)
                    return -10;
                else if (Board[0, col] == ai)
                    return +10;
            }
        }
        if (Board[0, 0] == Board[1, 1] && Board[1, 1] == Board[2, 2])
        {
            if (Board[0, 0] == human)
                return -10;
            if (Board[0, 0] == ai)
                return +10;
        }
        if (Board[0, 2] == Board[1, 1] && Board[1, 1] == Board[2, 0])
        {
            if (Board[0, 2] == human)
                return -10;
            if (Board[0, 2] == ai)
                return +10;
        }

        return 0;
    }

    public int MiniMax(int[,] Board, int depth, bool isMax)
    {
        int score = EvaluateScore(Board);

        if (score == +10 || score == -10 || isMoveLeft(Board) == false || depth == 0)
            return score;

        if (isMax)
        {
            int bestScore = -10000000;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Board[i, j] == empty)
                    {
                        Board[i, j] = ai;
                        int newScore = MiniMax(Board, depth - 1, false);
                        Board[i, j] = empty;
                        bestScore = Math.Max(newScore, bestScore);
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = 10000000;
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (Board[i, j] == empty)
                    {
                        Board[i, j] = human;
                        int newScore = MiniMax(Board, depth - 1, true);
                        Board[i, j] = empty;
                        bestScore = Math.Min(newScore, bestScore);
                    }
                }
            }
            return bestScore;
        }
    }

    public (int, int) FindBestMove(int[,] Board)
    {
        int bestValue = -10000000;
        int row = -1;
        int col = -1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Board[i, j] == empty)
                {
                    Board[i, j] = ai;
                    int value = MiniMax(Board, Constant.Difficulty, false);
                    Board[i, j] = empty;
                    if (value > bestValue)
                    {
                        bestValue = value;
                        row = i;
                        col = j;
                    }
                }
            }
        }
        return (row, col);
    }
}
