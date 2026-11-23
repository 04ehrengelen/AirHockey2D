
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public enum GoalType
    {
        PlayerGoal,
        AIGoal
    }

    [Header("Goal Settings")]
    public GoalType goalType;

    [Header("Game Manager")]
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Puck"))
            return;

        if (gameManager == null)
        {
            Debug.LogError("GameManager belum dihubungkan!");
            return;
        }

        if (gameManager.IsGameFinished())
            return;

        PuckController puck =
            other.GetComponent<PuckController>();

        if (puck == null)
        {
            Debug.LogError("PuckController tidak ditemukan!");
            return;
        }

        // Gawang kiri: AI mendapatkan poin
        if (goalType == GoalType.PlayerGoal)
        {
            gameManager.AddAIScore();
            Debug.Log("GOAL! AI mendapatkan poin.");
        }
        // Gawang kanan: Player mendapatkan poin
        else
        {
            gameManager.AddPlayerScore();
            Debug.Log("GOAL! Player mendapatkan poin.");
        }

        // Jika pertandingan selesai, hentikan puck
        if (gameManager.IsGameFinished())
        {
            puck.StopPuck();
        }
        else
        {
            // Jika belum selesai, spawn dan luncurkan kembali
            puck.ResetAndLaunch();
        }
    }
}
