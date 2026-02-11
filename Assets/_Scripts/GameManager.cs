using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public TMP_Text roundStatusText;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text newHighScoreText;
    public GameObject centerMessagePanel;
    public TMP_Text endMessageText;
    public Button restartButton;

    [Header("Scoring")]
    public int wallHitPoints = 1;
    public int goalPoints = 3;
    public float roundCompleteDisplaySeconds = 1.5f;

    private const string HighScoreKey = "HighScore";

    private int score;
    private int highScore;
    private bool roundTransitionInProgress;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        score = 0;
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (centerMessagePanel != null)
        {
            centerMessagePanel.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        SetRestartButtonVisible(false);
        SetNewHighScoreVisible(false);

        RefreshHud();
    }

    public void OnGoalHit()
    {
        if (roundTransitionInProgress)
        {
            return;
        }

        AddPoints(goalPoints);
        roundTransitionInProgress = true;
        StartCoroutine(HandleGoalHitRoutine());
    }

    public void OnProjectileHitWall()
    {
        if (roundTransitionInProgress)
        {
            return;
        }

        AddPoints(wallHitPoints);
    }

    public void OnLevelLoaded()
    {
        Goal.goalMet = false;
        roundTransitionInProgress = false;
        SetRestartButtonVisible(false);
        RefreshHud();
    }

    public void OnRestartButtonClicked()
    {
        StopAllCoroutines();
        roundTransitionInProgress = false;
        Goal.goalMet = false;

        score = 0;

        if (centerMessagePanel != null)
        {
            centerMessagePanel.SetActive(false);
        }

        SetRestartButtonVisible(false);
        SetNewHighScoreVisible(false);

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.RestartFromBeginning();
        }

        RefreshHud();
    }

    private IEnumerator HandleGoalHitRoutine()
    {
        bool isLastRound = LevelManager.Instance != null && LevelManager.Instance.IsLastLevel;

        if (!isLastRound)
        {
            if (centerMessagePanel != null)
            {
                centerMessagePanel.SetActive(false);
            }

            SetRestartButtonVisible(false);

            if (roundStatusText != null)
            {
                roundStatusText.text = "Round Completed";
            }

            yield return new WaitForSeconds(roundCompleteDisplaySeconds);

            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.NextLevel();
            }
        }
        else
        {
            if (centerMessagePanel != null)
            {
                centerMessagePanel.SetActive(true);
            }

            if (endMessageText != null)
            {
                endMessageText.text = "Game Completed";
            }

            SetRestartButtonVisible(true);
        }
    }

    private void SaveHighScoreIfNeeded()
    {
        if (score <= highScore)
        {
            return;
        }

        highScore = score;
        PlayerPrefs.SetInt(HighScoreKey, highScore);
        PlayerPrefs.Save();
        SetNewHighScoreVisible(true);
    }

    private void AddPoints(int points)
    {
        if (points <= 0)
        {
            return;
        }

        score += points;
        SaveHighScoreIfNeeded();
        RefreshHud();
    }

    private void RefreshHud()
    {
        if (LevelManager.Instance != null && roundStatusText != null)
        {
            int currentRound = LevelManager.Instance.CurrentLevelIndex + 1;
            int totalRounds = LevelManager.Instance.TotalLevels;
            roundStatusText.text = "Round " + currentRound + "/" + totalRounds;
        }

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    private void SetRestartButtonVisible(bool isVisible)
    {
        if (restartButton == null)
        {
            return;
        }

        restartButton.gameObject.SetActive(isVisible);
    }

    private void SetNewHighScoreVisible(bool isVisible)
    {
        if (newHighScoreText == null)
        {
            return;
        }

        newHighScoreText.gameObject.SetActive(isVisible);
        if (isVisible)
        {
            newHighScoreText.text = "New High Score!";
        }
    }
}
