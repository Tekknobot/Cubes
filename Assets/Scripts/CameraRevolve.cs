using UnityEngine;

public class CameraRevolve : MonoBehaviour
{
    private Vector3 targetPosition = Vector3.zero; // Default target position
    public float radius = 5f;   // Radius of the circular motion
    public float speed = 30f;   // Speed of automatic rotation in degrees per second
    public float mouseSensitivity = 0.2f; // Sensitivity for mouse movement
    public float verticalClamp = 85f; // Clamp angle for vertical rotation

    private float horizontalAngle = 0f;   // Current horizontal angle of the camera around the target
    private float verticalAngle = 20f;   // Current vertical angle of the camera
    private bool isDragging = false;     // Check if the mouse is being dragged

    void Update()
    {
        HandleMouseInput();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        // Check for left mouse button press and drag
        if (Input.GetMouseButtonDown(0))
        {
            // Perform a raycast to check if the mouse is over an object with the "Cube" tag
            if (!IsMouseOverCube())
            {
                isDragging = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Update angles based on mouse input if dragging
        if (isDragging)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            horizontalAngle -= mouseX; // Horizontal rotation
            verticalAngle = Mathf.Clamp(verticalAngle - mouseY, -verticalClamp, verticalClamp); // Clamp vertical rotation
        }
        else
        {
            // Automatic rotation when not dragging
            horizontalAngle += speed * Time.deltaTime;
        }
    }

    void UpdateCameraPosition()
    {
        // Calculate the new camera position
        float x = targetPosition.x + radius * Mathf.Cos(horizontalAngle * Mathf.Deg2Rad) * Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
        float z = targetPosition.z + radius * Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * Mathf.Cos(verticalAngle * Mathf.Deg2Rad);
        float y = targetPosition.y + radius * Mathf.Sin(verticalAngle * Mathf.Deg2Rad);

        // Update the camera's position and make it look at the target
        transform.position = new Vector3(x, y, z);
        transform.LookAt(targetPosition);
    }

    // Method to set the target position dynamically
    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
    }

    // Method to check if the mouse is over a "Cube" object
    private bool IsMouseOverCube()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.CompareTag("Cube");
        }
        return false;
    }
}
