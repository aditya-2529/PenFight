using TMPro;
using UnityEngine;

public class TurnUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text turnText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager =
            FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (gameManager == null)
        {
            return;
        }

        if (gameManager.IsGameOver)
        {
            return;
        }

        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (gameManager.CurrentPlayer ==
            GameManager.Player.Player1)
        {
            turnText.text =
                "PLAYER 1 TURN";
        }
        else
        {
            turnText.text =
                "PLAYER 2 TURN";
        }
    }
}