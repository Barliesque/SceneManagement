using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
	static private int _inst;
	
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

			var inst = property.GetHashCode();
			if (EditorGUI.EndChangeCheck() || _inst != inst) // || (value?.name ?? null) != sceneName.stringValue)
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
				_inst = inst;
			}
		}
		EditorGUI.EndProperty();
	}
}