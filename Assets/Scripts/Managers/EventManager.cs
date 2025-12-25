using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public static Action OnGameRestart;
    public static Action<GameConfig, Action<CardController>> OnStartGame;
    public static Action<bool> SetCardsInteractable;
    public static Action RevealAllCards;
    public static Action HideAllCards;
    public static Action ClearGrid;

    public static Action<float> UpdateTimer;
    public static Action<int> UpdateMoves;
    public static Action<int> UpdateScore;
    public static Action<int, float, int> ShowWinScreen;
    public static Action ShowLoseScreen;
    public static Action HideWinScreen;
    public static Action HideLoseScreen;
}
