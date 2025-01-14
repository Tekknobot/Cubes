using UnityEngine;

public class ButtonGenerator : MonoBehaviour
{
    public GameObject buttonPrefab; // Prefab for buttons (a Quad with Collider and ButtonAction)
    public GameObject cubeRoot; // The parent object of the Rubik's Cube
    public float buttonDistance = 2.0f; // Distance from the cube to place the buttons
    public float buttonSize = 1.0f; // Size of each button

    void Start()
    {
        Vector3 cubeCenter = cubeRoot.transform.position;

        // Create buttons for each face
        CreateButton("Top", cubeCenter + Vector3.up * (buttonDistance), Vector3.up);
        CreateButton("Bottom", cubeCenter + Vector3.down * (buttonDistance), Vector3.down);
        CreateButton("Front", cubeCenter + Vector3.forward * (buttonDistance), Vector3.forward);
        CreateButton("Back", cubeCenter + Vector3.back * (buttonDistance), Vector3.back);
        CreateButton("Left", cubeCenter + Vector3.left * (buttonDistance), Vector3.left);
        CreateButton("Right", cubeCenter + Vector3.right * (buttonDistance), Vector3.right);
    }

    void CreateButton(string name, Vector3 position, Vector3 normal)
    {
        // Instantiate the button prefab
        GameObject button = Instantiate(buttonPrefab, position, Quaternion.identity);
        //button.transform.localScale = new Vector3(buttonSize, buttonSize, 0.1f); // Set the size
        button.transform.up = normal; // Align button to face outward
        button.name = $"{name} Button";

        // Attach the ButtonAction script and initialize
        ButtonAction buttonAction = button.GetComponent<ButtonAction>();
        if (buttonAction == null)
        {
            buttonAction = button.AddComponent<ButtonAction>();
        }
        buttonAction.Initialize(name);
    }
}
