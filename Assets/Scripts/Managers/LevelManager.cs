using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static GameConfig SelectedLevelConfig { get; set; }

    private static LevelManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
