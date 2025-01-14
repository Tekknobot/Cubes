using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    private string buttonName;

    public void Initialize(string name)
    {
        buttonName = name;
    }

    void OnMouseDown()
    {
        // Call the appropriate rotation based on button name
        Debug.Log($"Button {buttonName} clicked!");

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
