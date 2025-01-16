using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RubiksCubeController : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spacing = 1.1f; // Spacing between cubes
    public float rotationSpeed = 2f; // Speed of rotation animation

    private CubeState[,,] cubes = new CubeState[3, 3, 3]; // Rubik's cube grid with state
    private Vector3 centerOffset;
    private List<string> scrambleMoves = new List<string>(); // Scramble moves for tracking state
    private bool isExecuting = false; // Flag to prevent overlapping executions
    private bool isScrambled = false; // Tracks whether the cube is scrambled

    private float lastClickTime = 0f; // Time of the last mouse click
    private float doubleClickThreshold = 0.3f; // Maximum time difference for a double-click
    private bool isDragging = false; // Tracks whether the user is dragging
    private Vector3 initialMousePosition; // Starting mouse position
    private GameObject selectedCube; // Cube that was clicked
    private Vector3Int rotationAxis; // Axis for rotation
    private int layerIndex; // Layer to rotate
    private List<(Vector3Int axis, int layerIndex, float angle)> rotationHistory = new List<(Vector3Int, int, float)>(); // Rotation history

    void Start()
    {
        Vector3 gridCenter = GenerateRubiksCube(); // Generate the Rubik's cube
        ShareCenterWithCamera(gridCenter); // Share the center with the CameraRevolve script
        StartCoroutine(ExecuteScramble());
    }

    void Update()
    {
        HandleMouseInput();
    }

    private float holdStartTime = 0f; // Track when the hold started
    private bool isHolding = false; // Track whether the user is holding the mouse button

    void HandleMouseInput()
    {
        // Prevent input during rotation
        if (isExecuting) return;

        // Detect mouse button down
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Cube"))
                {
                    // Start holding
                    isHolding = true;
                    holdStartTime = Time.time;
                }
            }
        }

        // Detect mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            // Reset holding state
            isHolding = false;
        }

        // Detect if held for 3 seconds or longer
        if (isHolding && Time.time - holdStartTime >= 3f)
        {
            // Trigger SolveCube function
            Debug.Log("Hold detected: Solving the cube...");
            SolveCube();

            // Reset holding state to prevent repeated triggering
            isHolding = false;
        }
    }

    public void OnDoubleClick(string faceName)
    {
        RubiksCubeController controller = FindObjectOfType<RubiksCubeController>();
        if (controller != null)
        {
            switch (faceName)
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


    public void RotateTopClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("U'");
        StartCoroutine(RotateFace(Vector3Int.up, 2, 90f));
    }

    public void RotateTopCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("U");
        StartCoroutine(RotateFace(Vector3Int.up, 2, -90f));
    }

    public void RotateBottomClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("D'");
        StartCoroutine(RotateFace(Vector3Int.up, 0, 90f));
    }

    public void RotateBottomCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("D");
        StartCoroutine(RotateFace(Vector3Int.up, 0, -90f));
    }

    public void RotateLeftClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("L'");
        StartCoroutine(RotateFace(Vector3Int.right, 0, -90f));
    }

    public void RotateLeftCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("L");
        StartCoroutine(RotateFace(Vector3Int.right, 0, 90f));
    }

    public void RotateRightClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("R'");
        StartCoroutine(RotateFace(Vector3Int.right, 2, 90f));
    }

    public void RotateRightCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("R");
        StartCoroutine(RotateFace(Vector3Int.right, 2, -90f));
    }

    public void RotateFrontClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("F'");
        StartCoroutine(RotateFace(Vector3Int.forward, 2, 90f));
    }

    public void RotateFrontCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("F");
        StartCoroutine(RotateFace(Vector3Int.forward, 2, -90f));
    }

    public void RotateBackClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("B'");
        StartCoroutine(RotateFace(Vector3Int.forward, 0, -90f));
    }

    public void RotateBackCounterClockwise()
    {
        if (isExecuting) return; // Prevent overlapping rotations
        scrambleMoves.Add("B");
        StartCoroutine(RotateFace(Vector3Int.forward, 0, 90f));
    }


    IEnumerator ExecuteScramble()
    {
        Debug.Log("Scrambling cube...");
        yield return ScrambleCube(20);
        Debug.Log("Scramble complete!");
        isScrambled = true; // Set state to scrambled
        isExecuting = false;
    }

    IEnumerator ScrambleCube(int moves)
    {
        string[] possibleMoves = { "U", "U'", "D", "D'", "F", "F'", "B", "B'", "L", "L'", "R", "R'" };

        for (int i = 0; i < moves; i++)
        {
            string move = possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Length)];
            scrambleMoves.Add(move);
            yield return PerformMove(move);
        }

        Debug.Log("Cube scrambled with moves: " + string.Join(", ", scrambleMoves));
    }

    IEnumerator PerformMove(string move)
    {
        Vector3Int axis;
        int layerIndex;
        float angle;

        switch (move)
        {
            case "U": axis = Vector3Int.up; layerIndex = 2; angle = 90f; break;
            case "U'": axis = Vector3Int.up; layerIndex = 2; angle = -90f; break;
            case "D": axis = Vector3Int.up; layerIndex = 0; angle = -90f; break;
            case "D'": axis = Vector3Int.up; layerIndex = 0; angle = 90f; break;
            case "F": axis = Vector3Int.forward; layerIndex = 2; angle = 90f; break;
            case "F'": axis = Vector3Int.forward; layerIndex = 2; angle = -90f; break;
            case "B": axis = Vector3Int.forward; layerIndex = 0; angle = -90f; break;
            case "B'": axis = Vector3Int.forward; layerIndex = 0; angle = 90f; break;
            case "L": axis = Vector3Int.right; layerIndex = 0; angle = -90f; break;
            case "L'": axis = Vector3Int.right; layerIndex = 0; angle = 90f; break;
            case "R": axis = Vector3Int.right; layerIndex = 2; angle = 90f; break;
            case "R'": axis = Vector3Int.right; layerIndex = 2; angle = -90f; break;
            default:
                Debug.LogError($"Invalid move: {move}");
                yield break;
        }

        yield return RotateFace(axis, layerIndex, angle); // Perform the rotation
    }

    IEnumerator RotateFace(Vector3Int axis, int layerIndex, float angle, bool isReset = false)
    {
        if (isExecuting && !isReset)
        {
            Debug.LogWarning("Rotation in progress. Ignoring new request.");
            yield break;
        }

        isExecuting = true;

        if (!isReset)
        {
            rotationHistory.Add((axis, layerIndex, angle));
            Debug.Log($"Rotation added to history: Axis {axis}, Layer {layerIndex}, Angle {angle}");
        }

        // Remaining rotation logic...
        List<CubeState> faceCubes = GetFaceCubes(axis, layerIndex);
        if (faceCubes == null || faceCubes.Count != 9)
        {
            Debug.LogError($"Incorrect number of cubes selected for axis {axis}, layer {layerIndex}");
            isExecuting = false;
            yield break;
        }

        GameObject pivot = new GameObject("Pivot");
        pivot.transform.parent = transform;
        pivot.transform.position = CalculatePivotPosition(axis, layerIndex);

        foreach (CubeState cubeState in faceCubes)
        {
            cubeState.CubeObject.transform.parent = pivot.transform;
        }

        Quaternion startRotation = pivot.transform.rotation;
        Quaternion endRotation = Quaternion.Euler((Vector3)axis * angle) * startRotation;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * rotationSpeed;
            pivot.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed);
            yield return null;
        }

        pivot.transform.rotation = endRotation;

        foreach (CubeState cubeState in faceCubes)
        {
            cubeState.CubeObject.transform.parent = transform;
            cubeState.UpdateRotation(axis, angle);
        }

        Destroy(pivot);
        UpdateGridAfterRotation(faceCubes);

        isExecuting = false;
        Debug.Log($"RotateFace completed: Axis {axis}, Layer {layerIndex}, Angle {angle}");
    }


    public void SolveCube()
    {
        if (isExecuting) return; // Prevent execution if already in progress

        StartCoroutine(ResetToOriginalState());
    }

    IEnumerator ResetToOriginalState()
    {
        if (rotationHistory.Count == 0)
        {
            Debug.Log("No rotation history to reverse.");
            yield break;
        }

        isExecuting = true; // Lock execution
        Debug.Log("Resetting cube to original state...");

        for (int i = rotationHistory.Count - 1; i >= 0; i--)
        {
            var (axis, layerIndex, angle) = rotationHistory[i];
            Debug.Log($"Reversing rotation: Axis {axis}, Layer {layerIndex}, Angle {-angle}");

            // Use the isReset flag to allow execution during reset
            yield return RotateFace(axis, layerIndex, -angle, true);
        }

        rotationHistory.Clear();
        Debug.Log("Cube reset to original state.");
        isExecuting = false;
    }



    private Vector3 CalculatePivotPosition(Vector3Int axis, int layerIndex)
    {
        // Start with the center of the cube
        Vector3 pivotPosition = Vector3.zero;

        // Adjust the pivot position based on the axis and layer index
        if (axis == Vector3Int.up) // Y-axis (Up/Down rotation)
        {
            pivotPosition = new Vector3(spacing, layerIndex * spacing, spacing) - centerOffset;
        }
        else if (axis == Vector3Int.right) // X-axis (Left/Right rotation)
        {
            pivotPosition = new Vector3(layerIndex * spacing, spacing, spacing) - centerOffset;
        }
        else if (axis == Vector3Int.forward) // Z-axis (Front/Back rotation)
        {
            pivotPosition = new Vector3(spacing, spacing, layerIndex * spacing) - centerOffset;
        }

        return pivotPosition;
    }

    void UpdateGridAfterRotation(List<CubeState> faceCubes)
    {
        CubeState[,,] newGrid = (CubeState[,,])cubes.Clone();

        foreach (CubeState cubeState in faceCubes)
        {
            // Calculate the new grid position
            Vector3 localPos = cubeState.CubeObject.transform.localPosition + centerOffset;
            Vector3Int newGridPosition = new Vector3Int(
                Mathf.RoundToInt(localPos.x / spacing),
                Mathf.RoundToInt(localPos.y / spacing),
                Mathf.RoundToInt(localPos.z / spacing)
            );

            // Clamp to valid indices
            newGridPosition = new Vector3Int(
                Mathf.Clamp(newGridPosition.x, 0, 2),
                Mathf.Clamp(newGridPosition.y, 0, 2),
                Mathf.Clamp(newGridPosition.z, 0, 2)
            );

            // Update cube state
            cubeState.UpdateGridPosition(newGridPosition);
            newGrid[newGridPosition.x, newGridPosition.y, newGridPosition.z] = cubeState;

            Debug.Log($"Cube moved to {newGridPosition}");
        }

        cubes = newGrid; // Replace old grid
    }


    List<CubeState> GetFaceCubes(Vector3Int axis, int layerIndex)
    {
        List<CubeState> faceCubes = new List<CubeState>();

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    CubeState cubeState = cubes[x, y, z];
                    if (cubeState != null)
                    {
                        // Check if the cube is on the specified layer
                        if ((axis == Vector3Int.up && cubeState.GridPosition.y == layerIndex) ||
                            (axis == Vector3Int.forward && cubeState.GridPosition.z == layerIndex) ||
                            (axis == Vector3Int.right && cubeState.GridPosition.x == layerIndex))
                        {
                            faceCubes.Add(cubeState);
                        }
                    }
                }
            }
        }

        return faceCubes;
    }

    Vector3 GenerateRubiksCube()
    {
        // Calculate the center offset so the grid is centered at the origin
        centerOffset = new Vector3((2 * spacing) / 2, (2 * spacing) / 2, (2 * spacing) / 2);

        // Loop through the 3x3x3 grid and instantiate cubes at correct positions
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    // Calculate the local position for each cube
                    Vector3 localPosition = new Vector3(x * spacing, y * spacing, z * spacing) - centerOffset;

                    // Instantiate the cube prefab and set its local position
                    GameObject cubePiece = Instantiate(cubePrefab, transform);
                    cubePiece.transform.localPosition = localPosition;

                    // Store the cube in the grid with its initial state
                    cubes[x, y, z] = new CubeState(cubePiece, new Vector3Int(x, y, z));
                }
            }
        }

        // Return the grid center position in world space
        return transform.position;
    }

    void ShareCenterWithCamera(Vector3 gridCenter)
    {
        CameraRevolve cameraRevolve = FindObjectOfType<CameraRevolve>();
        if (cameraRevolve != null)
        {
            cameraRevolve.SetTargetPosition(gridCenter);
        }
    }

    public class CubeState
    {
        public GameObject CubeObject { get; private set; }
        public Vector3Int GridPosition { get; private set; }
        private Quaternion localRotation;

        public CubeState(GameObject cubeObject, Vector3Int gridPosition)
        {
            CubeObject = cubeObject;
            GridPosition = gridPosition;
            localRotation = cubeObject.transform.localRotation;
        }

        public bool IsOnLayer(Vector3Int axis, int layerIndex)
        {
            if (axis == Vector3Int.up) return GridPosition.y == layerIndex;
            if (axis == Vector3Int.forward) return GridPosition.z == layerIndex;
            if (axis == Vector3Int.right) return GridPosition.x == layerIndex;
            return false;
        }

        public void UpdateRotation(Vector3Int axis, float angle)
        {
            localRotation = Quaternion.Euler((Vector3)axis * angle) * localRotation;
            CubeObject.transform.localRotation = localRotation;
        }

        public void UpdateGridPosition(Vector3Int newGridPosition)
        {
            GridPosition = newGridPosition;
        }
    }
}
