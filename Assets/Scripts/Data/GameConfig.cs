using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Card Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Level Info")]
    public int levelNumber;

    [Header("Grid Settings")]
    public int gridColumns = 4;
    public int gridRows = 4;
    public float gridSpacing = 10f;
    public float gridPadding = 20f;


    [Header("Game Settings")]
    public float flipDuration = 0.3f;
    public float mismatchDelay = 1f;
    public bool useTimer = true;
    public float gameDuration = 120f;
    public bool showPreview = true; 
    public float previewDuration = 3f; 


    [Header("Card Settings")]
    public CardData[] availableCards;

    [Header("Scoring")]
    public int matchPoints = 100;
    public int mismatchPenalty = 10;
}
