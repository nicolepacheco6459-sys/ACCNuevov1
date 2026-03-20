using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
    public List<ChestSaveData> chestsState;
}

[Serializable]

public class ChestSaveData
{
    public string chestID;
    public bool isOpened;
}