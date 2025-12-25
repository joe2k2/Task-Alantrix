using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgress
{
    public int levelNumber;
    public bool isUnlocked;
    public bool isCompleted;
    public int highScore;
    public float bestTime;
}

[Serializable]
public class GameProgressData
{
    public List<LevelProgress> levelProgresses;
    public int currentLevel;
    public DateTime lastPlayedDate;

    public GameProgressData()
    {
        levelProgresses = new List<LevelProgress>();
        currentLevel = 1;
        lastPlayedDate = DateTime.Now;
    }
}
