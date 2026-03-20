using UnityEngine;
using Unity.Cinemachine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class SaveController : MonoBehaviour
{
    private string saveLocation;

    [Header("Referencias")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineConfiner2D confiner;

    private Chest[] chests;
    private List<ChestSaveData> chestsState;

    void Awake()
    {
        // Auto-asignaciones de seguridad
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (inventoryController == null)
            inventoryController = FindFirstObjectByType<InventoryController>();

        if (confiner == null)
            confiner = FindFirstObjectByType<CinemachineConfiner2D>();
    }

    void Start()
    {
        InitializedComponents();
        LoadGame();
    }

    private void InitializedComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        chests = FindObjectsByType<Chest>(FindObjectsSortMode.None);

    }

    public void SaveGame()
    {
        if (player == null || inventoryController == null || confiner == null)
        {
            Debug.LogError("SaveController: Faltan referencias.");
            return;
        }

        SaveData saveData = new SaveData
        {
            playerPosition = player.position,
            mapBoundary = confiner.BoundingShape2D != null 
                          ? confiner.BoundingShape2D.name 
                          : "",
            inventorySaveData = inventoryController.GetInventoryItems(),
            chestsState = GetChestsState()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log("Juego guardado en: " + saveLocation);
    }

    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestsState = new List<ChestSaveData>();
        
        foreach(Chest chest in chests)
        {
            ChestSaveData chestData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            };
            chestsState.Add(chestData);
        }
        return chestsState;
    }
    public void LoadGame()
    {
        if (!File.Exists(saveLocation))
        {
            Debug.Log("No existe guardado, creando uno nuevo...");
            SaveGame();
            return;
        }

        SaveData saveData = JsonUtility.FromJson<SaveData>(
            File.ReadAllText(saveLocation));

        if (player != null)
            player.position = saveData.playerPosition;

        if (!string.IsNullOrEmpty(saveData.mapBoundary))
        {
            GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);

            if (boundaryObject != null)
            {
                PolygonCollider2D savedMapBoundry =
                    boundaryObject.GetComponent<PolygonCollider2D>();
                    FindFirstObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();

                if (savedMapBoundry != null && confiner != null)
                {
                    confiner.BoundingShape2D = savedMapBoundry;
                    confiner.InvalidateBoundingShapeCache();
                    // Notifica al MapController iluminar el area del mapa si está disponible
                    MapController_Manual.Instance?.HighlighArea(saveData.mapBoundary);
                    MapController_Dynamic.Instance?.GenerateMap();
                    MapController_Dynamic.Instance?.UpdateCurrentArea(saveData.mapBoundary);
                }
            }
            else
            {
                LoadChestsState(saveData.chestsState);
                Debug.LogWarning("SaveController: No se encontró el objeto de límite guardado.");
            }
        }

        //if (inventoryController != null)
            ///inventoryController.SetInventoryItems(saveData.inventorySaveData);
        if (saveData.inventorySaveData != null)
            inventoryController.SetInventoryItems(saveData.inventorySaveData);
        Debug.Log("Juego cargado correctamente");

    }

    private void LoadChestsState(List<ChestSaveData> chestsState)
    {
        foreach(Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestsState.FirstOrDefault(c => c.chestID == chest.ChestID);

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();

        MapController_Dynamic.Instance?.GenerateMap();
    }
}