using UnityEditor;
using UnityEngine;

namespace KissMyAssets.VisualNovelCore.Editor
{
    public static class ConnectionDrawer
    {
        public static void DrawConnectionTo(Vector2 from, Vector2 to, Color? color = null, float thickness = 3f)
        {
            float dx = Mathf.Abs(from.x - to.x);
            Vector2 startTangent = from + Vector2.right * Mathf.Max(40f, dx * 0.5f);
            Vector2 endTangent = to + Vector2.left * Mathf.Max(40f, dx * 0.5f);

            Color drawColor = color ?? (EditorGUIUtility.isProSkin ? new Color(0.60f, 0.85f, 1f, 0.95f) : new Color(0.00f, 0.45f, 0.90f, 0.95f));
            Handles.DrawBezier(from, to, startTangent, endTangent, drawColor, null, thickness);
        }
    }
}