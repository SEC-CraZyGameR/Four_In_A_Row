using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class FourInARow : MonoBehaviour
{
    public static FourInARow Instance = null;

    private Player Player;

    public List<RectTransform> CellList;
    public List<RectTransform> TopPosCellList;
    public List<Button> Buttons;
    public GameObject[,] PieceContainer = new GameObject[6, 7];
    public RectTransform[,] GameBoard = new RectTransform[6, 7];
    public int[,] Board = new int[6, 7];

    public GameObject redPiece;
    public GameObject bluePiece;


    private float durationtime = 0;
    int row = 6;
    int col = 7;
    int row_count = 6;
    int col_count = 7;
    private int player_piece = 1;
    private int ai_piece = 2;
    private int window_length = 4;
    public List<TextMeshProUGUI> board;

    int previousWinState = 1;
    private void Start()
    {

        UiManager.Instance.action += GameOverBtnsCallBack;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        int cellindex = 0;

        for (int j = 0; j < 7; j++)
        {
            for (int i = 0; i < 6; i++)
            {
                GameBoard[i, j] = CellList[cellindex++];
                Board[i, j] = 0;
            }
        }

        int buttonIndex = 0;
        for (int i = 0; i < Buttons.Count; i++)
        {
            var current = buttonIndex;
            Buttons[i].onClick.AddListener(() => ButtonCallBack(current));
            buttonIndex++;
        }
        SetPlayer(Player.Human);
    }

    public void ResetGame()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                Destroy(PieceContainer[i, j]);
                Board[i, j] = 0;
            }
        }
        if (previousWinState == 0)
        {
            int player = UnityEngine.Random.Range(1, 3);

            if (player == 1)
            {
                SetPlayer(Player.Human);
                UiManager.Instance.DisplayWhoseTurn(Player.Human);
            }
            else
            {
                SetPlayer(Player.Ai);
                StartCoroutine(AiTurn());
                UiManager.Instance.DisplayWhoseTurn(Player.Ai);
            }
        }
        else
        {
            if (previousWinState == 1)
            {
                StartCoroutine(AiTurn());
                SetPlayer(Player.Ai);
                UiManager.Instance.DisplayWhoseTurn(Player.Ai);
            }
            else
            {
                SetPlayer(Player.Human);
                UiManager.Instance.DisplayWhoseTurn(Player.Human);
            }
        }
    }

    private void GameOverBtnsCallBack(int tag, int index)
    {
        if (tag == 2)
        {
            if (index == 1)
                FourInARowUI.Instance.PlayAgainCallBack();
            else if (index == 0)
                SceneManager.LoadScene(0);
        }
    }
    private void OnDestroy()
    {
        UiManager.Instance.action -= GameOverBtnsCallBack;
    }
    private void SetPlayer(Player _player)
    {
        Player = _player;
    }

    private void ButtonCallBack(int colNumber)
    {
        if (!GameManager.Instance.isGameStart || GameManager.Instance.isGameOver)
            return;

        if (Player == Player.Human)
        {
            if (IsValidLocation(Board, colNumber))
            {
                int rowNumber = 0;
                rowNumber = GetNextOpenRow(Board, colNumber);

                RectTransform celltransform = GameBoard[rowNumber, colNumber];
                GameObject piece = Instantiate(redPiece, TopPosCellList[colNumber].transform);
                PieceContainer[rowNumber, colNumber] = piece;
                piece.GetComponent<RectTransform>().position = TopPosCellList[colNumber].transform.position;
                durationtime = (row - rowNumber) / 10f;
                piece.GetComponent<RectTransform>().DOMove(celltransform.position, durationtime).SetEase(Ease.OutBounce);
                SoundManager.Instance.PlaySound(SoundManager.Instance.dropPiece);
                DropPiece(Board, rowNumber, colNumber, (int)Player.Human);
                if (WinningMove(Board, (int)Player.Human))
                {
                    List<(int, int)> winPositions = GetWinPosition(Board, (int)Player.Human);
                    for (int i = 0; i < winPositions.Count; i++)
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
                        //PieceContainer[winPositions[i].Item1, winPositions[i].Item2].GetComponent<Image>().color = Color.red;
                        PieceContainer[winPositions[i].Item1, winPositions[i].Item2].transform.DOScale(new Vector2(.6f, .6f), 1.0f).SetLoops(-1);
                    }
                    GameManager.Instance.GameOver();
                    UiManager.Instance.GameOverMenu(2);
                    UiManager.Instance.DisPlayWinner(Player.Human);
                    previousWinState = 1;
                    return;
                }
                if (!HasMove())
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
                    GameManager.Instance.GameOver();
                    UiManager.Instance.GameOverMenu(2);
                    UiManager.Instance.DisPlayWinner(0);
                    previousWinState = 0;
                    return;
                }
                SetPlayer(Player.Ai);
                UiManager.Instance.DisplayWhoseTurn(Player.Ai);
                if (!GameManager.Instance.isGameStart || GameManager.Instance.isGameOver)
                    return;
                StartCoroutine(AiTurn());
            }
        }
    }

    #region Connect4 AI
    public bool HasMove()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if (Board[i, j] == 0)
                    return true;
            }
        }
        return false;
    }

    public IEnumerator AiTurn()
    {
        yield return new WaitForSeconds(1f);
        var result = Minimax(Board, Constant.Difficulty, -Mathf.Infinity, Mathf.Infinity, true);
        int colNumber = (int)result.Item1;
        int rowNumber = 0;
        if (IsValidLocation(Board, colNumber))
        {
            rowNumber = GetNextOpenRow(Board, colNumber);

            //Debug.Log(rowNumber + "     " + colNumber + "       " + result.Item2);
        }
        else
        {
            Debug.Log("Else");
            for (int j = 0; j < 7; j++)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (Board[i, j] == 0)
                    {
                        rowNumber = i;
                        colNumber = j;
                        break;
                    }
                }
            }
            //List<int> validLocation = GetValidLocations(Board);
            //for (int i = 0; i < validLocation.Count; i++)
            //{
            //    int row = GetNextOpenRow(Board, i);
            //    var B_copy = Board.Clone() as int[,];
            //    DropPiece(B_copy, row, i, 1);
            //    if (WinningMove(B_copy, 1))
            //    {
            //        Debug.Log("Should Play....");
            //        rowNumber = row;
            //        colNumber = i;
            //        break;
            //    }
            //}
        }
        RectTransform celltransform = GameBoard[rowNumber, colNumber];
        GameObject piece = Instantiate(bluePiece, TopPosCellList[colNumber].transform);
        PieceContainer[rowNumber, colNumber] = piece;
        piece.GetComponent<RectTransform>().position = TopPosCellList[colNumber].transform.position;
        durationtime = (row - rowNumber) / 10f;
        piece.GetComponent<RectTransform>().DOMove(celltransform.position, durationtime).SetEase(Ease.OutBounce);
        SoundManager.Instance.PlaySound(SoundManager.Instance.dropPiece);
        DropPiece(Board, rowNumber, colNumber, (int)Player.Ai);
        if (WinningMove(Board, (int)Player.Ai))
        {
            List<(int, int)> winPositions = GetWinPosition(Board, (int)Player.Ai);
            for (int i = 0; i < winPositions.Count; i++)
            {
                //PieceContainer[winPositions[i].Item1, winPositions[i].Item2].GetComponent<Image>().color = Color.red;
                PieceContainer[winPositions[i].Item1, winPositions[i].Item2].transform.DOScale(new Vector2(.6f, .6f), 1.0f).SetLoops(-1);
            }
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            GameManager.Instance.GameOver();
            UiManager.Instance.GameOverMenu(2);
            UiManager.Instance.DisPlayWinner(Player.Ai);
            previousWinState = 2;
            yield break;

        }
        if (!HasMove())
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            GameManager.Instance.GameOver();
            UiManager.Instance.DisPlayWinner(0);
            UiManager.Instance.GameOverMenu(2);
            previousWinState = 0;
            yield break;
        }
        SetPlayer(Player.Human);
        UiManager.Instance.DisplayWhoseTurn(Player.Human);
        //Debug.Log(rowNumber + "     " + colNumber + "       " + result.Item2);

    }
    public void DropPiece(int[,] Board, int row, int col, int piece)
    {
        Board[row, col] = piece;
    }

    public bool IsValidLocation(int[,] Board, int col)
    {
        return Board[row_count - 1, col] == 0;
    }

    public int GetNextOpenRow(int[,] Board, int col)
    {
        for (int r = 0; r < row_count; r++)
        {
            if (Board[r, col] == 0)
                return r;
        }
        return 0;
    }

    public bool WinningMove(int[,] Board, int piece)
    {
        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 0; r < row_count; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r, c + 1] == piece &&
                    Board[r, c + 2] == piece &&
                    Board[r, c + 3] == piece
                    )
                {
                    return true;
                }
            }
        }
        //Vertical Check............
        for (int c = 0; c < col_count; c++)
        {
            for (int r = 0; r < row_count - 3; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r + 1, c] == piece &&
                    Board[r + 2, c] == piece &&
                    Board[r + 3, c] == piece
                    )
                {
                    return true;
                }
            }
        }
        //Check Diagonal Right..............
        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 0; r < row_count - 3; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r + 1, c + 1] == piece &&
                    Board[r + 2, c + 2] == piece &&
                    Board[r + 3, c + 3] == piece
                    )
                {
                    return true;
                }
            }
        }
        //Check Diagonal Left............
        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 3; r < row_count; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r - 1, c + 1] == piece &&
                    Board[r - 2, c + 2] == piece &&
                    Board[r - 3, c + 3] == piece
                    )
                {
                    return true;
                }
            }
        }

        return false;
    }

    public List<(int, int)> GetWinPosition(int[,] Board, int piece)
    {
        List<(int, int)> positions = new List<(int, int)>();

        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 0; r < row_count; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r, c + 1] == piece &&
                    Board[r, c + 2] == piece &&
                    Board[r, c + 3] == piece
                    )
                {
                    positions.Add((r, c));
                    positions.Add((r, c + 1));
                    positions.Add((r, c + 2));
                    positions.Add((r, c + 3));

                    return positions;
                }
            }
        }
        //Vertical Check............
        for (int c = 0; c < col_count; c++)
        {
            for (int r = 0; r < row_count - 3; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r + 1, c] == piece &&
                    Board[r + 2, c] == piece &&
                    Board[r + 3, c] == piece
                    )
                {
                    positions.Add((r, c));
                    positions.Add((r + 1, c));
                    positions.Add((r + 2, c));
                    positions.Add((r + 3, c));
                    return positions;
                }
            }
        }
        //Check Diagonal Right..............
        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 0; r < row_count - 3; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r + 1, c + 1] == piece &&
                    Board[r + 2, c + 2] == piece &&
                    Board[r + 3, c + 3] == piece
                    )
                {
                    positions.Add((r, c));
                    positions.Add((r + 1, c + 1));
                    positions.Add((r + 2, c + 2));
                    positions.Add((r + 3, c + 3));
                    return positions;
                }
            }
        }
        //Check Diagonal Left............
        for (int c = 0; c < col_count - 3; c++)
        {
            for (int r = 3; r < row_count; r++)
            {
                if (Board[r, c] == piece &&
                    Board[r - 1, c + 1] == piece &&
                    Board[r - 2, c + 2] == piece &&
                    Board[r - 3, c + 3] == piece
                    )
                {
                    positions.Add((r, c));
                    positions.Add((r - 1, c + 1));
                    positions.Add((r - 2, c + 2));
                    positions.Add((r - 3, c + 3));
                    return positions;
                }
            }
        }

        return positions;
    }

    public int EvaluateWindow(int[] window, int piece)
    {
        int score = 0;
        int opp_piece = player_piece;
        if (piece == player_piece)
            opp_piece = ai_piece;

        if (window.Count(n => n == piece) == 4)
        {
            score += 100;
        }
        else if (window.Count(n => n == piece) == 3 && window.Count(n => n == 0) == 1)
            score += 50;
        else if (window.Count(n => n == piece) == 2 && window.Count(n => n == 0) == 2)
            score += 20;

        if (window.Count(n => n == opp_piece) == 3 && window.Count(n => n == 0) == 1)
            score -= 40;

        return score;
    }

    public int ScorePosition(int[,] Board, int piece)
    {
        int Score = 0;
        List<int> center_array = new List<int>();
        for (int i = 0; i < row_count; i++)
        {
            center_array.Add(Board[i, col_count / 2]);
        }
        int centerCount = center_array.ToArray().Count(n => n == piece);
        Score += centerCount * 30;

        for (int r = 0; r < row_count; r++)
        {
            List<int> row_array = new List<int>();
            for (int i = 0; i < col_count; i++)
            {
                row_array.Add(Board[r, i]);
            }
            for (int c = 0; c < col_count - 3; c++)
            {
                int[] window = row_array.ToArray().Slice(c, c + window_length);
                Score += EvaluateWindow(window, piece);
            }
        }


        for (int c = 0; c < col_count; c++)
        {
            List<int> col_array = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                col_array.Add(Board[i, c]);
            }
            for (int r = 0; r < row_count - 3; r++)
            {
                int[] window = col_array.ToArray().Slice(r, r + window_length);
                Score += EvaluateWindow(window, piece);
            }
        }

        for (int r = 0; r < row_count - 3; r++)
        {
            for (int c = 0; c < col_count - 3; c++)
            {
                List<int> window = new List<int>();
                for (int i = 0; i < window_length; i++)
                {
                    window.Add(Board[r + i, c + i]);
                }
                Score += EvaluateWindow(window.ToArray(), piece);
            }
        }
        for (int r = 0; r < row_count - 3; r++)
        {
            for (int c = 0; c < col_count - 3; c++)
            {
                List<int> window = new List<int>();
                for (int i = 0; i < window_length; i++)
                {
                    window.Add(Board[r + 3 - i, c + i]);
                }
                Score += EvaluateWindow(window.ToArray(), piece);
            }
        }
        return Score;
    }

    public bool IsTerminalNode(int[,] Board)
    {

        return WinningMove(Board, player_piece) ||
            WinningMove(Board, ai_piece) ||
            GetValidLocations(Board).Count == 0;
    }

    public List<int> GetValidLocations(int[,] Board)
    {
        List<int> validLocations = new List<int>();
        for (int c = 0; c < col_count; c++)
        {
            if (IsValidLocation(Board, c))
            {
                validLocations.Add(c);
            }
        }
        return validLocations;
    }

    public (int?, double) Minimax(int[,] Board, int depth, double alpha, double beta, bool isMax)
    {
        int? something = null;
        List<int> validLocations = GetValidLocations(Board);
        bool isTerminal = IsTerminalNode(Board);
        if (depth == 0 || isTerminal)
        {
            if (isTerminal)
            {
                if (WinningMove(Board, ai_piece))
                    return (something, 100000000000000);
                else if (WinningMove(Board, player_piece))
                    return (something, -100000000000000);
                else
                    return (something, 0);
            }
            else
            {
                return (something, ScorePosition(Board, ai_piece));
            }
        }

        if (isMax)
        {
            double value = -Mathf.Infinity;
            int column = validLocations[UnityEngine.Random.Range(0, validLocations.Count)];
            for (int col = 0; col < validLocations.Count; col++)
            {
                int row = GetNextOpenRow(Board, col);
                var B_copy = Board.Clone() as int[,];
                DropPiece(B_copy, row, col, ai_piece);
                var newScore = Minimax(B_copy, depth - 1, alpha, beta, false);
                if (newScore.Item2 > value)
                {
                    value = newScore.Item2;
                    column = col;
                }
                alpha = Math.Max(alpha, value);
                if (alpha >= beta)
                    break;
            }
            return (column, value);
        }
        else
        {
            double value = Mathf.Infinity;
            int column = validLocations[UnityEngine.Random.Range(0, validLocations.Count)];
            for (int col = 0; col < validLocations.Count; col++)
            {
                int row = GetNextOpenRow(Board, col);
                var B_copy = Board.Clone() as int[,];
                DropPiece(B_copy, row, col, player_piece);
                var newScore = Minimax(B_copy, depth - 1, alpha, beta, true);
                if (newScore.Item2 < value)
                {
                    value = newScore.Item2;
                    column = col;
                }
                beta = Math.Min(beta, value);
                if (alpha >= beta)
                    break;
            }
            return (column, value);
        }
    }
    #endregion
}
