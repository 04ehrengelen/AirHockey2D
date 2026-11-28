
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "MainGame";

    [Header("Game Mode Buttons")]
    public GameObject playerVsAIButton;
    public GameObject playerVsPlayerButton;

    private bool isModeMenuOpen = false;

    private void Start()
    {
        // Sembunyikan pilihan mode saat Main Menu dibuka
        HideGameModes();
    }

    public void ToggleGameModes()
    {
        isModeMenuOpen = !isModeMenuOpen;

        playerVsAIButton.SetActive(isModeMenuOpen);
        playerVsPlayerButton.SetActive(isModeMenuOpen);

        Debug.Log(
            isModeMenuOpen
            ? "Pilihan Game Mode dibuka."
            : "Pilihan Game Mode ditutup."
        );
    }

    public void HideGameModes()
    {
        isModeMenuOpen = false;

        if (playerVsAIButton != null)
            playerVsAIButton.SetActive(false);

        if (playerVsPlayerButton != null)
            playerVsPlayerButton.SetActive(false);
    }

    public void PlayAsAI()
    {
        GameSettings.gameMode = GameMode.PlayerVsAI;

        Debug.Log("Mode dipilih: Player vs AI");

        LoadGame();
    }

    public void PlayAsPlayer()
    {
        GameSettings.gameMode = GameMode.PlayerVsPlayer;

        Debug.Log("Mode dipilih: Player vs Player");

        LoadGame();
    }

    private void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar dari Air Hockey 2D.");
        Application.Quit();
    }
}
