using UnityEngine;

public class ArenaOutOfBounds : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PenController pen =
            other.GetComponent<PenController>();

        if (pen == null)
        {
            return;
        }

        Debug.Log(
            pen.GetPlayerName() +
            " is out of the arena!"
        );

        if (gameManager != null)
        {
            gameManager.PlayerEliminated(
                pen.GetGameManagerPlayer()
            );
        }
    }
}