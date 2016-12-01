using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapboxConvenience))]
[CanEditMultipleObjects]
public class MapboxConvenienceEditor : Editor
{
	SerializedProperty _token;

	void OnEnable()
	{
		_token = serializedObject.FindProperty("_token");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		if (string.IsNullOrEmpty(_token.stringValue))
		{
			EditorGUILayout.HelpBox("You must have an access token!", MessageType.Error);
			if (GUILayout.Button("Get a token from mapbox.com for free"))
			{
				Application.OpenURL("https://www.mapbox.com/studio/account/tokens/");
			}
		}

		EditorGUILayout.PropertyField(_token, new GUIContent("Token"));
		serializedObject.ApplyModifiedProperties();
	}
}
