using UnityEngine;

public static class GlobalHelper
{
    public static string GenerateUniqueID(GameObject obj)
    {
        string sceneName = obj.scene.name;
        Vector3 p = obj.transform.position;
        return $"{sceneName}_{p.x:F2}_{p.y:F2}_{p.z:F2}";

    }
}
