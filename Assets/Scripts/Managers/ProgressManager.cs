using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class LevelProgressData
{
    public int selectedLevel = 1;
    public List<int> completedLevels = new List<int>();
    public List<int> unlockedLevels = new List<int>();
}

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager instance;
    public static ProgressManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ProgressManager");
                instance = go.AddComponent<ProgressManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private LevelProgressData progressData;
    private string saveFilePath;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "level_progress.json");
        LoadProgress();

        if (!IsLevelUnlocked(1))
        {
            UnlockLevel(1);
        }
    }

    private void LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            progressData = JsonUtility.FromJson<LevelProgressData>(json);
            Debug.Log("Progress loaded from: " + saveFilePath);
        }
        else
        {
            progressData = new LevelProgressData();
            Debug.Log("No save file found, created new progress");
        }
    }

    private void SaveProgress()
    {
        string json = JsonUtility.ToJson(progressData, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Progress saved to: " + saveFilePath);
    }

    public void SetSelectedLevel(int levelNumber)
    {
        progressData.selectedLevel = levelNumber;
        SaveProgress();
    }

    public int GetSelectedLevel()
    {
        return progressData.selectedLevel;
    }

    public void CompleteLevel(int levelNumber)
    {
        if (!progressData.completedLevels.Contains(levelNumber))
        {
            progressData.completedLevels.Add(levelNumber);
        }

        UnlockLevel(levelNumber + 1);
        SaveProgress();
    }

    public void UnlockLevel(int levelNumber)
    {
        if (!progressData.unlockedLevels.Contains(levelNumber))
        {
            progressData.unlockedLevels.Add(levelNumber);
            SaveProgress();
        }
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        return progressData.completedLevels.Contains(levelNumber);
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        return progressData.unlockedLevels.Contains(levelNumber);
    }

    public void ResetProgress()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        progressData = new LevelProgressData();
        UnlockLevel(1);
        Debug.Log("Progress reset");
    }
}
