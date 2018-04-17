using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapController map = (MapController)target;
        if (GUILayout.Button("Load Maps"))
        {
            map.ReloadMaps();
        }
    }
}
