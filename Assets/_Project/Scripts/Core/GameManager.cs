using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum Player
    {
        Player1,
        Player2
    }

    [Header("Player References")]
    [SerializeField] private Rigidbody player1Rb;
    [SerializeField] private Rigidbody player2Rb;

    [Header("Game Settings")]
    [SerializeField] private float stopSpeedThreshold = 0.05f;
    [SerializeField] private float stopCheckDelay = 0.5f;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text winnerText;

    private Player currentPlayer;

    private bool shotInProgress = false;
    private bool gameOver = false;

    private float stoppedTimer = 0f;

    // Starting positions
    private Vector3 player1StartPosition;
    private Vector3 player2StartPosition;

    // Starting rotations
    private Quaternion player1StartRotation;
    private Quaternion player2StartRotation;

    public Player CurrentPlayer => currentPlayer;

    public bool IsGameOver => gameOver;

    public bool IsShotInProgress => shotInProgress;

    private void Awake()
    {
        // Remember initial Player 1 position
        if (player1Rb != null)
        {
            player1StartPosition =
                player1Rb.position;

            player1StartRotation =
                player1Rb.rotation;
        }

        // Remember initial Player 2 position
        if (player2Rb != null)
        {
            player2StartPosition =
                player2Rb.position;

            player2StartRotation =
                player2Rb.rotation;
        }
    }

    private void Start()
    {
        StartNewMatch();
    }

    private void Update()
    {
        // Temporary testing shortcut.
        // Later this will be replaced by the Rematch button.
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (gameOver)
            {
                RestartMatch();
            }
        }

        if (gameOver)
        {
            return;
        }

        if (shotInProgress)
        {
            CheckCurrentPlayerStopped();
        }
    }

    // --------------------------------------------------
    // MATCH START
    // --------------------------------------------------

    private void StartNewMatch()
    {
        currentPlayer = Player.Player1;

        shotInProgress = false;
        gameOver = false;

        stoppedTimer = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Debug.Log("Game Started!");
        Debug.Log("Player 1 Turn");
    }

    // --------------------------------------------------
    // TURN SYSTEM
    // --------------------------------------------------

    public bool CanPlayerShoot(Player player)
    {
        if (gameOver)
        {
            return false;
        }

        if (shotInProgress)
        {
            return false;
        }

        return player == currentPlayer;
    }

    public void ShotStarted()
    {
        if (gameOver)
        {
            return;
        }

        shotInProgress = true;

        stoppedTimer = 0f;

        Debug.Log(
            currentPlayer +
            " shot started!"
        );
    }

    private void CheckCurrentPlayerStopped()
    {
        Rigidbody currentRb =
            GetCurrentPlayerRigidbody();

        if (currentRb == null)
        {
            return;
        }

        float speed =
            currentRb.linearVelocity.magnitude;

        if (speed <= stopSpeedThreshold)
        {
            stoppedTimer +=
                Time.deltaTime;

            if (stoppedTimer >=
                stopCheckDelay)
            {
                ShotFinished();
            }
        }
        else
        {
            stoppedTimer = 0f;
        }
    }

    private void ShotFinished()
    {
        shotInProgress = false;

        stoppedTimer = 0f;

        Debug.Log(
            currentPlayer +
            " shot finished."
        );

        SwitchTurn();
    }

    private void SwitchTurn()
    {
        if (currentPlayer ==
            Player.Player1)
        {
            currentPlayer =
                Player.Player2;
        }
        else
        {
            currentPlayer =
                Player.Player1;
        }

        Debug.Log(
            currentPlayer +
            " Turn"
        );
    }

    private Rigidbody GetCurrentPlayerRigidbody()
    {
        if (currentPlayer ==
            Player.Player1)
        {
            return player1Rb;
        }

        return player2Rb;
    }

    // --------------------------------------------------
    // GAME OVER
    // --------------------------------------------------

    public void PlayerEliminated(
        Player eliminatedPlayer)
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;

        shotInProgress = false;

        stoppedTimer = 0f;

        Player winner;

        if (eliminatedPlayer ==
            Player.Player1)
        {
            winner = Player.Player2;
        }
        else
        {
            winner = Player.Player1;
        }

        Debug.Log("GAME OVER!");

        Debug.Log(
            winner +
            " WINS!"
        );

        ShowWinner(winner);
    }

    private void ShowWinner(Player winner)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (winnerText != null)
        {
            winnerText.text =
                winner.ToString().ToUpper() +
                " WINS!";
        }
    }

    // --------------------------------------------------
    // REMATCH
    // --------------------------------------------------

    public void RestartMatch()
    {
        Debug.Log("Restarting Match...");

        ResetPlayer(
            player1Rb,
            player1StartPosition,
            player1StartRotation
        );

        ResetPlayer(
            player2Rb,
            player2StartPosition,
            player2StartRotation
        );

        ResetPenController(
            player1Rb
        );

        ResetPenController(
            player2Rb
        );

        StartNewMatch();
    }

    private void ResetPlayer(
        Rigidbody rb,
        Vector3 position,
        Quaternion rotation)
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        rb.transform.position =
            position;

        rb.transform.rotation =
            rotation;

        rb.Sleep();
    }

    private void ResetPenController(
        Rigidbody rb)
    {
        if (rb == null)
        {
            return;
        }

        PenController pen =
            rb.GetComponent<PenController>();

        if (pen != null)
        {
            pen.ResetForNewMatch();
        }
    }
}