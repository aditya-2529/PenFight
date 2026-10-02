using UnityEngine;

public class PenOutOfBounds : MonoBehaviour
{
    [Header("Arena Bounds")]
    [SerializeField] private float minX = -5.5f;
    [SerializeField] private float maxX = 5.5f;

    [SerializeField] private float minZ = -3.5f;
    [SerializeField] private float maxZ = 3.5f;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [Header("Player")]
    [SerializeField] private GameManager.Player player;

    private bool eliminated = false;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<GameManager>();
        }
    }

    private void Update()
    {
        if (eliminated)
        {
            return;
        }

        CheckOutOfBounds();
    }

    private void CheckOutOfBounds()
    {
        Vector3 position =
            transform.position;

        bool outsideX =
            position.x < minX ||
            position.x > maxX;

        bool outsideZ =
            position.z < minZ ||
            position.z > maxZ;

        if (outsideX || outsideZ)
        {
            EliminatePlayer();
        }
    }

    private void EliminatePlayer()
    {
        eliminated = true;

        Debug.Log(
            player +
            " is out of the arena!"
        );

        if (gameManager != null)
        {
            gameManager.PlayerEliminated(
                player
            );
        }
    }
}