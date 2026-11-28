using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject settingsPanel;

    [Header("Main Menu References")]
    public GameObject titleText;
    public GameObject playButton;
    public GameObject settingsButton;
    public GameObject quitButton;
    public GameObject playerVsAIButton;
    public GameObject playerVsPlayerButton;

    [Header("Sensitivity Settings")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;

    [Header("AI Difficulty Settings")]
    public TMP_Dropdown difficultyDropdown;

    private void Start()
    {
        settingsPanel.SetActive(false);

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = GameSettings.sensitivity;
            SetSensitivity(sensitivitySlider.value);
        }

        if (difficultyDropdown != null)
        {
            difficultyDropdown.value = GameSettings.difficulty;
            SetDifficulty(GameSettings.difficulty);
        }
    }

    public void OpenSettings()
    {
    settingsPanel.SetActive(true);

    titleText.SetActive(false);
    playButton.SetActive(false);
    settingsButton.SetActive(false);
    quitButton.SetActive(false);

    playerVsAIButton.SetActive(false);
    playerVsPlayerButton.SetActive(false);
    }

    public void CloseSettings()
    {
    settingsPanel.SetActive(false);

    titleText.SetActive(true);
    playButton.SetActive(true);
    settingsButton.SetActive(true);
    quitButton.SetActive(true);

    playerVsAIButton.SetActive(true);
    playerVsPlayerButton.SetActive(true);
    }

    public void SetSensitivity(float value)
    {
        GameSettings.sensitivity = value;

        if (sensitivityValueText != null)
            sensitivityValueText.text = value.ToString("0");

        Debug.Log("Sensitivity tersimpan: " + value);
    }

    public void SetDifficulty(int value)
    {
        GameSettings.difficulty = value;

        string difficulty = "Medium";

        switch (value)
        {
            case 0:
                difficulty = "Easy";
                break;

            case 1:
                difficulty = "Medium";
                break;

            case 2:
                difficulty = "Hard";
                break;
        }

        Debug.Log("AI Difficulty tersimpan: " + difficulty);
    }
}