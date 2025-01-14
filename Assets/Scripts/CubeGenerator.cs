using UnityEngine;

public class CubeGenerator : MonoBehaviour
{
    void Start()
    {
        CubeGenerator generator = GetComponent<CubeGenerator>();
        GameObject cube = generator.GenerateCube();

        // Save as prefab (manually or through Unity editor)
    }

    public Color[] faceColors = new Color[]
    {
        Color.white, // Top face
        Color.yellow, // Bottom face
        Color.green, // Front face
        Color.blue, // Back face
        Color.red, // Left face
        new Color(1f, 0.5f, 0f) // Orange (Right face)
    };

    public GameObject GenerateCube()
    {
        // Create an empty GameObject for the cube
        GameObject cube = new GameObject("RubiksCubePiece");

        // Add 6 faces as child planes
        for (int i = 0; i < 6; i++)
        {
            // Create a face
            GameObject face = GameObject.CreatePrimitive(PrimitiveType.Quad);
            face.name = $"Face_{i}";
            face.transform.SetParent(cube.transform);

            // Position and rotate the face based on its index
            switch (i)
            {
                case 0: // Top
                    face.transform.localPosition = new Vector3(0, 0.5f, 0);
                    face.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    break;
                case 1: // Bottom
                    face.transform.localPosition = new Vector3(0, -0.5f, 0);
                    face.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                    break;
                case 2: // Front
                    face.transform.localPosition = new Vector3(0, 0, 0.5f);
                    face.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 3: // Back
                    face.transform.localPosition = new Vector3(0, 0, -0.5f);
                    face.transform.localRotation = Quaternion.Euler(0, 180, 0);
                    break;
                case 4: // Left
                    face.transform.localPosition = new Vector3(-0.5f, 0, 0);
                    face.transform.localRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case 5: // Right
                    face.transform.localPosition = new Vector3(0.5f, 0, 0);
                    face.transform.localRotation = Quaternion.Euler(0, 90, 0);
                    break;
            }

            // Assign a color to the face
            Renderer renderer = face.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.color = faceColors[i];
        }

        return cube;
    }
}
