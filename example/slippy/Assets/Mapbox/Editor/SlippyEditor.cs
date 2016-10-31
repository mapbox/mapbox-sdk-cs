using UnityEditor;
using UnityEngine;

using Mapbox;
using Mapbox.Map;

[CustomEditor(typeof(Slippy))]
[CanEditMultipleObjects]
public class SlippyEditor : Editor
{
    // Access token.
    private SerializedProperty token;

    // Map limits.
    private SerializedProperty south;
    private SerializedProperty west;
    private SerializedProperty north;
    private SerializedProperty east;

    // Zoom level.
    private SerializedProperty zoom;

    // Tile edge size.
    private SerializedProperty edge;

    void OnEnable()
    {
        token = serializedObject.FindProperty("Token");

        south = serializedObject.FindProperty("South");
        west = serializedObject.FindProperty("West");
        north = serializedObject.FindProperty("North");
        east = serializedObject.FindProperty("East");

        zoom = serializedObject.FindProperty("Zoom");

        edge = serializedObject.FindProperty("Edge");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(token, new GUIContent("Token"));

        if (token.stringValue == Slippy.DebugToken)
        {
            EditorGUILayout.HelpBox("This is a debug token, do not use in production!", MessageType.Warning);

            if (GUILayout.Button("Get a token from mapbox.com for free"))
            {
                Application.OpenURL("https://www.mapbox.com/pricing/");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Southwest coordinate");
        south.doubleValue = EditorGUILayout.DelayedDoubleField(south.doubleValue);
        west.doubleValue = EditorGUILayout.DelayedDoubleField(west.doubleValue);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Northeast coordinate");
        north.doubleValue = EditorGUILayout.DelayedDoubleField(north.doubleValue);
        east.doubleValue = EditorGUILayout.DelayedDoubleField(east.doubleValue);
        EditorGUILayout.EndHorizontal();

        var bounds = new GeoCoordinateBounds(
            new GeoCoordinate(south.doubleValue, west.doubleValue),
            new GeoCoordinate(north.doubleValue, east.doubleValue));

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Center coordinate");
        EditorGUILayout.LabelField(bounds.Center.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.IntSlider(zoom, 0, 20, new GUIContent("Zoom"));

        var tileCount = TileCover.Get(bounds, zoom.intValue).Count;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile count", tileCount.ToString());

        if (tileCount > Map<RasterTile>.TileMax)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Too many tiles!", MessageType.Error);
        }

        EditorGUILayout.Space();
        edge.floatValue = EditorGUILayout.FloatField("Tile edge", edge.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
