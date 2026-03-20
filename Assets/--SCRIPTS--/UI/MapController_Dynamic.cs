using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MapController_Dynamic : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform mapParent;
    public GameObject areaPrefab;
    public RectTransform playerIcon;

    [Header("Colours")]
    public Color defaultColour = Color.gray;
    public Color currentAreaColour = Color.white;

    [Header("Map Settings")]
    public GameObject MapBounds;
    public PolygonCollider2D initialArea;
    public float mapScale = 10f;

    private PolygonCollider2D[] mapAreas;
    private Dictionary<string, RectTransform> uiAreas = new Dictionary<string, RectTransform>();

    public static MapController_Dynamic Instance { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of MapController_Dynamic detected. Destroying duplicate.");
            Destroy(gameObject);
        }

        mapAreas = MapBounds.GetComponentsInChildren<PolygonCollider2D>();
    }

    //Generar Mapa
    public void GenerateMap()
    {
        PolygonCollider2D currentArea = initialArea;

        ClearMap();

        foreach (PolygonCollider2D area in mapAreas)
        {
            // Create Area UI
            CreateAreaUI(area, area == currentArea);
        }

    }
    //-----------------
    //Limpiar Mapa
    //-----------------
    private void ClearMap()
    {
        foreach (Transform child in mapParent)
        {
            Destroy(child.gameObject);
        }
        uiAreas.Clear();
    }

    private void CreateAreaUI(PolygonCollider2D area, bool isCurrent)
    {
        // Instanciar el prefab para imagen
        GameObject areaImage = Instantiate(areaPrefab, mapParent);
        RectTransform rectTransform = areaImage.GetComponent<RectTransform>();

        // Get Bounds
        Bounds bounds = area.bounds;

        // Escalar imagen UI según el tamaño del área
        rectTransform.sizeDelta = new Vector2(bounds.size.x * mapScale, bounds.size.y * mapScale);
        rectTransform.anchoredPosition = bounds.center * mapScale;

        // Mantener color basandose en el actual o no
        Image img = areaImage.GetComponent<Image>();
        if (img != null)
            img.color = isCurrent ? currentAreaColour : defaultColour;

        // Agregar a diccionario para referencia futura
        uiAreas[area.name] = rectTransform;
    }

    //-----------------
    //Actualizar Area actual
    //-----------------

    public void UpdateCurrentArea(string newCurrentArea)
    {
        foreach (KeyValuePair<string, RectTransform> entry in uiAreas)
        {
            Image img = entry.Value.GetComponent<Image>();
            if (img != null)
                img.color = entry.Key == newCurrentArea ? currentAreaColour : defaultColour;
        }

        MovePlayerIcon(newCurrentArea);
    }

    //-----------------
    //Mover icono del jugador
    //-----------------
    private void MovePlayerIcon(string newCurrentArea)
    {
        if (uiAreas.TryGetValue(newCurrentArea, out RectTransform areaUI))
        {
            playerIcon.anchoredPosition = areaUI.anchoredPosition;
        }
        else
        {
            Debug.LogWarning($"Area '{newCurrentArea}' not found in UI areas.");
        }
    }

    
}
