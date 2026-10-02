using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PenController : MonoBehaviour
{
    public enum Player
    {
        Player1,
        Player2
    }

    [Header("Player")]
    [SerializeField] private Player player;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private Slider powerBar;

    [Header("Flick Settings")]
    [SerializeField] private float maxPower = 0.25f;
    [SerializeField] private float maxDragDistance = 3f;
    [Header("Spin Settings")]
    [SerializeField] private float spinStrength = 1.5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float minimumPower = 0.1f;

    private Camera mainCamera;

    private bool isAiming = false;

    private Vector3 aimStartWorldPosition;

    private GameManager gameManager;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        gameManager = FindFirstObjectByType<GameManager>();

        if (aimLine != null)
        {
            aimLine.enabled = false;
        }

        if (powerBar != null)
        {
            powerBar.value = 0f;
        }
    }

    private void Update()
    {
        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryStartAiming();
        }

        if (Mouse.current.leftButton.isPressed && isAiming)
        {
            UpdateAim();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isAiming)
        {
            ReleaseShot();
        }
    }
    private void TryStartAiming()
    {
        if (gameManager == null)
        {
            return;
        }

        GameManager.Player gameManagerPlayer;

        if (player == Player.Player1)
        {
            gameManagerPlayer =
                GameManager.Player.Player1;
        }
        else
        {
            gameManagerPlayer =
                GameManager.Player.Player2;
        }

        // Check whether this player's turn
        if (!gameManager.CanPlayerShoot(gameManagerPlayer))
        {
            return;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit[] hits =
            Physics.RaycastAll(ray);

        foreach (RaycastHit hit in hits)
        {
            PenController clickedPen =
                hit.collider.GetComponentInParent<PenController>();

            if (clickedPen == this)
            {
                isAiming = true;

                aimStartWorldPosition =
                    transform.position;

                rb.linearVelocity =
                    Vector3.zero;

                rb.angularVelocity =
                    Vector3.zero;

                if (aimLine != null)
                {
                    aimLine.enabled = true;
                }

                return;
            }
        }
    }
    private void UpdateAim()
    {
        Vector3 currentWorldPosition =
            GetMouseWorldPosition();

        Vector3 dragVector =
            currentWorldPosition -
            aimStartWorldPosition;

        dragVector.y = 0f;

        dragVector =
            Vector3.ClampMagnitude(
                dragVector,
                maxDragDistance
            );

        Vector3 shotDirection =
            -dragVector.normalized;

        float power =
            dragVector.magnitude /
            maxDragDistance;

        power = Mathf.Clamp01(power);
        if (power < minimumPower)
        {
            CancelAim();
            return;
        }

        if (powerBar != null)
        {
            powerBar.value = power;
        }

        if (aimLine != null)
        {
            float trajectoryLength =
                dragVector.magnitude * 3f;

            int pointCount = 20;

            aimLine.positionCount =
                pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float t =
                    i / (float)(pointCount - 1);

                Vector3 point =
                    transform.position +
                    shotDirection *
                    trajectoryLength *
                    t;

                point.y =
                    transform.position.y;

                aimLine.SetPosition(i, point);
            }
        }
    }

    private void ReleaseShot()
    {
        Vector3 currentWorldPosition =
            GetMouseWorldPosition();

        Vector3 dragVector =
            currentWorldPosition -
            aimStartWorldPosition;

        dragVector.y = 0f;

        dragVector =
            Vector3.ClampMagnitude(
                dragVector,
                maxDragDistance
            );

        if (dragVector.magnitude < 0.05f)
        {
            CancelAim();
            return;
        }

        Vector3 shotDirection =
            -dragVector.normalized;

         float power =
            dragVector.magnitude /
            maxDragDistance;

        power = Mathf.Clamp01(power);
        if (power < minimumPower)
        {
            CancelAim();
            return;
        }

        rb.AddForce(
            shotDirection *
            power *
            maxPower,
            ForceMode.Impulse
        );
        float spinDirection =
            Vector3.Dot(
                transform.right,
                shotDirection
            );

        rb.AddTorque(
            Vector3.up *
            spinDirection *
            power *
            spinStrength,
            ForceMode.Impulse
        );
        isAiming = false;

        if (aimLine != null)
        {
            aimLine.enabled = false;
        }

        if (powerBar != null)
        {
            powerBar.value = 0f;
        }

        if (gameManager != null)
        {
            gameManager.ShotStarted();
        }
        Debug.Log(
            player +
            " Shot | Power: " +
            power.ToString("F2") +
            " | Drag: " +
            dragVector.magnitude.ToString("F2") +
            " | Direction: " +
            shotDirection
        );
    }

    private void CancelAim()
    {
        isAiming = false;

        if (aimLine != null)
        {
            aimLine.enabled = false;
        }

        if (powerBar != null)
        {
            powerBar.value = 0f;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Ray ray =
            mainCamera.ScreenPointToRay(
                mousePosition
            );

        Plane groundPlane =
            new Plane(
                Vector3.up,
                transform.position
            );

        if (groundPlane.Raycast(
            ray,
            out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
    public string GetPlayerName()
    {
        return player.ToString();
    }

    public GameManager.Player GetGameManagerPlayer()
    {
        if (player == Player.Player1)
        {
            return GameManager.Player.Player1;
        }

        return GameManager.Player.Player2;
    }
    public void ResetForNewMatch()
    {
        isAiming = false;

        if (aimLine != null)
        {
            aimLine.enabled = false;
        }

        if (powerBar != null)
        {
            powerBar.value = 0f;
        }
    }
}