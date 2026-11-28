
using UnityEngine;

public enum GameMode
{
    PlayerVsAI,
    PlayerVsPlayer
}

public static class GameSettings
{
    public static float sensitivity = 15f;

    public static int difficulty = 1;

    public static GameMode gameMode = GameMode.PlayerVsAI;
}
