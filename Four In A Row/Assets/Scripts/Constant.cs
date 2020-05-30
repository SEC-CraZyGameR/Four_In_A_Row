using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public enum Player
{
    Human = 1,
    Ai = 2
}
public enum CellType
{
    Human = 1,
    Ai = 2,
    Empty = 0
}

public enum GameResult
{
    Human = 1,
    Ai = -1,
    tie = 0
}

public enum GameWinDirection
{
    right,
    left,
    up,
    down,
    rightDiagonalUp,
    rightDiagonalDown,
    leftDiagonalUp,
    leftDiagonalDown
}
enum Panel
{
    TicTacToe = 1,
    FourInARow = 2,
    GameOverButtons
}

public static class Constant
{
    public static int row = 6;
    public static int col = 7;
    public static int Difficulty;

    public static T[] Slice<T>(this T[] source, int start, int end)
    {
        // Handles negative ends.
        if (end < 0)
        {
            end = source.Length + end;
        }
        int len = end - start;

        // Return new array.
        T[] res = new T[len];
        for (int i = 0; i < len; i++)
        {
            res[i] = source[i + start];
        }
        return res;
    }
    public static void SetActiveFalse(params GameObject[] gameobjects)
    {
        for (int i = 0; i < gameobjects.Length; i++)
        {
            gameobjects[i].SetActive(false);
        }
    }
    public static void SetActiveTrue(params GameObject[] gameobjects)
    {
        for (int i = 0; i < gameobjects.Length; i++)
        {
            gameobjects[i].SetActive(true);
        }
    }
}
