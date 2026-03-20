using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MapController_Manual : MonoBehaviour
{
    public static MapController_Manual Instance { get; set; }

    public GameObject mapPanel;
    public Color highlighColour = Color.yellow;
    public Color dimmedColour = new Color(1f, 1f, 1f, 0.5f);

    public RectTransform playerIconTransform;

    private List<Image> mapImages = new List<Image>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple instances of MapController_Manual detected. Destroying duplicate.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        mapImages = mapPanel.GetComponentsInChildren<Image>().ToList();
    }

    public void HighlighArea(string areaName)
    {
        foreach (Image area in mapImages)
        {
            area.color = dimmedColour;
        }

        Image currentArea = mapImages.Find(x => x.name == areaName);
    
        if (currentArea != null)
        {
            currentArea.color = highlighColour;

            playerIconTransform.position = currentArea.GetComponent<RectTransform>().position;
        }
        else
        {
            Debug.LogWarning($"Area '{areaName}' not found in map images.");
        }
    }
}
