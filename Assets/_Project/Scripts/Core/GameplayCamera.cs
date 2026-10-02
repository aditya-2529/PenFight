using UnityEngine;

public class GameplayCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Vector3 cameraPosition =
        new Vector3(0f, 10f, -7f);

    [SerializeField] private Vector3 cameraRotation =
        new Vector3(55f, 0f, 0f);

    [SerializeField] private float fieldOfView = 50f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        ApplyCameraSettings();
    }

    private void ApplyCameraSettings()
    {
        transform.position =
            cameraPosition;

        transform.rotation =
            Quaternion.Euler(
                cameraRotation
            );

        if (cam != null)
        {
            cam.fieldOfView =
                fieldOfView;
        }
    }
}