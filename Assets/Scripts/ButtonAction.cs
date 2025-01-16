using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    private string buttonName; // The name of the button
    private float lastClickTime = 0f; // Time of the last click
    private float doubleClickThreshold = 0.3f; // Maximum time difference for a double-click
    private bool isWaitingForDoubleClick = false; // Flag to check for a double-click

    public void Initialize(string name)
    {
        buttonName = name;
    }

    void OnMouseDown()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold && isWaitingForDoubleClick)
        {
            // Double-click detected: Rotate counterclockwise
            isWaitingForDoubleClick = false; // Reset flag
            Debug.Log($"Button {buttonName} double-clicked for counterclockwise rotation!");

            RubiksCubeController controller = FindObjectOfType<RubiksCubeController>();
            if (controller != null)
            {
                switch (buttonName)
                {
                    case "Top": controller.RotateTopCounterClockwise(); break;
                    case "Bottom": controller.RotateBottomCounterClockwise(); break;
                    case "Front": controller.RotateFrontCounterClockwise(); break;
                    case "Back": controller.RotateBackCounterClockwise(); break;
                    case "Left": controller.RotateLeftCounterClockwise(); break;
                    case "Right": controller.RotateRightCounterClockwise(); break;
                }
            }
        }
        else
        {
            // Start waiting for a potential double-click
            isWaitingForDoubleClick = true;
            Invoke(nameof(HandleSingleClick), doubleClickThreshold);
        }

        // Update the time of the last click
        lastClickTime = Time.time;
    }

    void HandleSingleClick()
    {
        if (!isWaitingForDoubleClick) return; // If a double-click occurred, skip single-click handling
        isWaitingForDoubleClick = false; // Reset flag

        Debug.Log($"Button {buttonName} clicked for clockwise rotation!");

        RubiksCubeController controller = FindObjectOfType<RubiksCubeController>();
        if (controller != null)
        {
            switch (buttonName)
            {
                case "Top": controller.RotateTopClockwise(); break;
                case "Bottom": controller.RotateBottomClockwise(); break;
                case "Front": controller.RotateFrontClockwise(); break;
                case "Back": controller.RotateBackClockwise(); break;
                case "Left": controller.RotateLeftClockwise(); break;
                case "Right": controller.RotateRightClockwise(); break;
            }
        }
    }
}
