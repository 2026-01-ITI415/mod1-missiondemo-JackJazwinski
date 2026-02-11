using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Settings")]
    public GameObject[] levelPrefabs;

    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;

    public int CurrentLevelIndex
    {
        get { return currentLevelIndex; }
    }

    public int TotalLevels
    {
        get { return levelPrefabs != null ? levelPrefabs.Length : 0; }
    }

    public bool IsLastLevel
    {
        get { return currentLevelIndex >= TotalLevels - 1; }
    }

    private void Awake()
    {
        // Singleton pattern (easy access from anywhere)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevel(currentLevelIndex);
    }
    
    public void LoadLevel(int index)
    {
        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("LevelManager has no level prefabs assigned.");
            return;
        }

        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("LevelManager received invalid level index: " + index);
            return;
        }

        // Reset goal state whenever a new level is loaded
        Goal.goalMet = false;

        // Clean up all projectiles left in the scene
        CleanupProjectiles();

        // Housekeeping – destroy old level
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        // Load new level
        currentLevelInstance = Instantiate(levelPrefabs[index]);
        currentLevelIndex = index;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelLoaded();
        }
    }

    private void CleanupProjectiles()
    {
        // Find all Projectile objects in the scene and destroy them
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile proj in projectiles)
        {
            Destroy(proj.gameObject);
        }
    }
    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;

        if (nextIndex < levelPrefabs.Length)
        {
            LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("Game Complete!");
        }
    }
    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }

    public void RestartFromBeginning()
    {
        LoadLevel(0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
