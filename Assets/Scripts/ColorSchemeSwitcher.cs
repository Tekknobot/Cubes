using System.Collections.Generic;
using UnityEngine;

public class ColorSchemeSwitcher : MonoBehaviour
{
    public Material[] cubeMaterials; // Assign materials for each face in the Inspector
    private Dictionary<int, Color[]> colorSchemes;
    private int currentScheme = 1; // Tracks the current color scheme
    private List<Renderer> highlightedRenderers = new List<Renderer>(); // Stores renderers of highlighted cells
    private Material originalMaterial; // Original material used before highlighting

    private float lastClickTime = 0f; // Time of the last mouse click
    private float doubleClickThreshold = 0.3f; // Time threshold for double-clicks

    void Start()
    {
        // Initialize color schemes (all properties are the same for each scheme)
        colorSchemes = new Dictionary<int, Color[]>
        {
            { 1, new Color[] { Color.white, Color.yellow, Color.red, new Color(1f, 0.5f, 0f), Color.green, Color.blue } },
            { 2, new Color[] { HexToColor("#FF007F"), HexToColor("#00FFFF"), HexToColor("#7FFF00"), HexToColor("#8A2BE2"), HexToColor("#FF4500"), HexToColor("#DFFF00") } },
            { 3, new Color[] { HexToColor("#2E2B5F"), HexToColor("#564D90"), HexToColor("#8C52FF"), HexToColor("#C580FF"), HexToColor("#1C1C3A"), HexToColor("#4A4AFF") } },
            { 4, new Color[] { HexToColor("#39FF14"), HexToColor("#F21B3F"), HexToColor("#FEFF00"), HexToColor("#F20089"), HexToColor("#4D00FF"), HexToColor("#00F9FF") } },
            { 5, new Color[] { HexToColor("#FF6F61"), HexToColor("#6B4226"), HexToColor("#FFD700"), HexToColor("#6495ED"), HexToColor("#8A2BE2"), HexToColor("#228B22") } },
            { 6, new Color[] { HexToColor("#FFC1CC"), HexToColor("#FF69B4"), HexToColor("#FFB6C1"), HexToColor("#FF85CB"), HexToColor("#FF1493"), HexToColor("#FF6EB4") } },
            { 7, new Color[] { HexToColor("#154406"), HexToColor("#2D6A4F"), HexToColor("#52B788"), HexToColor("#76C893"), HexToColor("#40916C"), HexToColor("#1B4332") } },
            { 8, new Color[] { HexToColor("#031D44"), HexToColor("#04395E"), HexToColor("#70A9A1"), HexToColor("#A3D5D3"), HexToColor("#E5F9DB"), HexToColor("#2B8A9D") } },
            { 9, new Color[] { HexToColor("#8B0000"), HexToColor("#FF4500"), HexToColor("#FF8C00"), HexToColor("#FFD700"), HexToColor("#FF6347"), HexToColor("#A52A2A") } }
        };

        // Apply the initial color scheme
        ApplyColorScheme(currentScheme);
    }

    void Update()
    {
        // Detect double-click
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            float currentTime = Time.time;
            if (currentTime - lastClickTime <= doubleClickThreshold)
            {
                HandleDoubleClick();
            }
            lastClickTime = currentTime;
        }
    }

    void HandleDoubleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Cube"))
            {
                Debug.Log("Double-clicked a Cube! Nothing happens...");
            }
            else
            {
                Debug.Log("Double-clicked a non-Cube object. Cycling color scheme...");
                CycleScheme();
            }
        }
        else
        {
            Debug.Log("Double-clicked empty space. Cycling color scheme...");
            CycleScheme();
        }
    }

    void CycleScheme()
    {
        currentScheme = (currentScheme % colorSchemes.Count) + 1; // Increment and wrap around
        ApplyColorScheme(currentScheme);
        Debug.Log($"Switched to color scheme {currentScheme}");
    }

    void ApplyColorScheme(int key)
    {
        if (!colorSchemes.ContainsKey(key))
        {
            Debug.LogError($"No color scheme found for key {key}");
            return;
        }

        var colors = colorSchemes[key];

        // Ensure all highlighted cells are reset to their original material
        ResetHighlightedCells();

        for (int i = 0; i < cubeMaterials.Length; i++)
        {
            if (i < colors.Length)
            {
                cubeMaterials[i].SetColor("_Color", colors[i]);           // Set base color
                cubeMaterials[i].SetColor("_EmissionColor", colors[i]);  // Set emission color
                cubeMaterials[i].SetColor("_BackColor", colors[i]);      // Set back color
            }
        }

        Debug.Log($"Applied color scheme {key}");
    }

    void ResetHighlightedCells()
    {
        foreach (Renderer renderer in highlightedRenderers)
        {
            if (renderer != null)
            {
                renderer.material = originalMaterial; // Restore the original material
            }
        }

        // Clear the highlighted list
        highlightedRenderers.Clear();
    }

    // Helper method to convert hex to Color
    Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        else
        {
            Debug.LogError($"Invalid hex color: {hex}");
            return Color.white;
        }
    }
}
