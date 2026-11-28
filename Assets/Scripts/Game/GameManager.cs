
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Score")]
    public int playerScore = 0;
    public int aiScore = 0;
    public int winningScore = 5;

    [Header("UI References")]
    public TMP_Text playerScoreText;
    public TMP_Text aiScoreText;
    public TMP_Text gameStatusText;

    [Header("Player Name UI")]
    public TMP_Text leftPlayerNameText;
    public TMP_Text rightPlayerNameText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text winnerText;

    private bool gameFinished = false;

    private void Start()
    {
        Time.timeScale = 1f;

        UpdateScoreUI();
        UpdateModeUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameStatusText != null)
            gameStatusText.text = "AIR HOCKEY";
    }

    private void UpdateModeUI()
    {
        if (GameSettings.gameMode == GameMode.PlayerVsAI)
        {
            if (leftPlayerNameText != null)
                leftPlayerNameText.text = "PLAYER 1";

            if (rightPlayerNameText != null)
                rightPlayerNameText.text = "AI";
        }
        else
        {
            if (leftPlayerNameText != null)
                leftPlayerNameText.text = "PLAYER 1";

            if (rightPlayerNameText != null)
                rightPlayerNameText.text = "PLAYER 2";
        }
    }

    public void AddPlayerScore()
    {
        if (gameFinished) return;

        playerScore++;

        UpdateScoreUI();
        CheckWinner();
    }

    public void AddAIScore()
    {
        if (gameFinished) return;

        aiScore++;

        UpdateScoreUI();
        CheckWinner();
    }

    private void UpdateScoreUI()
    {
        if (playerScoreText != null)
            playerScoreText.text = playerScore.ToString();

        if (aiScoreText != null)
            aiScoreText.text = aiScore.ToString();
    }

    private void CheckWinner()
    {
        if (playerScore >= winningScore)
        {
            FinishGame("PLAYER 1 WINS!");
        }
        else if (aiScore >= winningScore)
        {
            if (GameSettings.gameMode == GameMode.PlayerVsAI)
                FinishGame("AI WINS!");
            else
                FinishGame("PLAYER 2 WINS!");
        }
    }

    private void FinishGame(string message)
    {
        gameFinished = true;

        if (winnerText != null)
            winnerText.text = message;

        if (gameStatusText != null)
            gameStatusText.text = "GAME OVER";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Debug.Log(message);
    }

    public bool IsGameFinished()
    {
        return gameFinished;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Kembali ke Main Menu.");

        SceneManager.LoadScene("MainMenu");
    }
}
