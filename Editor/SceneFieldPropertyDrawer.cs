using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
	override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, GUIContent.none, property);
		var sceneAsset = property.FindPropertyRelative("sceneAsset");
		var sceneName = property.FindPropertyRelative("sceneName");
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		if (sceneAsset != null)
		{
			EditorGUI.BeginChangeCheck();
			var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
			
			if (EditorGUI.EndChangeCheck()) // || (value?.name ?? null) != sceneName.stringValue)
			{
				sceneAsset.objectReferenceValue = value;
				if (sceneAsset.objectReferenceValue != null)
				{
					var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
					var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
					var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
					scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
					sceneName.stringValue = scenePath;
				}
			}
		}
		EditorGUI.EndProperty();
	}
}