using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Example game data
    public int Score { get; private set; }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Init();
    }

    private void Init()
    {
        // Basic initialization logic
        Score = 0;
        IsInitialized = true;

    }

    // Example: add score
    public void AddScore(int amount)
    {
        Score += amount;
        Debug.Log($"Score: {Score}");
    }
}
