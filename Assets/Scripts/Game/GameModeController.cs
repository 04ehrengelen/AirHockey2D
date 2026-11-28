
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [Header("Player 2 Components")]
    public AIController aiController;
    public HandTrackingReceiverP2 player2Receiver;

    private void Awake()
    {
        if (aiController == null)
        {
            Debug.LogError("AIController belum dihubungkan!");
            return;
        }

        if (player2Receiver == null)
        {
            Debug.LogError("HandTrackingReceiverP2 belum dihubungkan!");
            return;
        }

        if (GameSettings.gameMode == GameMode.PlayerVsAI)
        {
            aiController.enabled = true;
            player2Receiver.enabled = false;

            Debug.Log("Game Mode: Player vs AI");
        }
        else
        {
            aiController.enabled = false;
            player2Receiver.enabled = true;

            Debug.Log("Game Mode: Player vs Player");
        }
    }
}
