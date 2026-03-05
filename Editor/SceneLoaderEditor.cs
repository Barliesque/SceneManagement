using Barliesque.InspectorTools.Editor;
using UnityEditor;
using UnityEngine;

namespace Barliesque.SceneManagement.Editor
{
	[CustomEditor(typeof(SceneLoader))]
	public class SceneLoaderEditor : EditorBase<SceneGroup>
	{
		private ListEditor _sceneGroups;

		override protected void OnEnabled(SceneGroup inst)
		{
			UpdateGroupsList();
		}

		private void UpdateGroupsList()
		{
			_sceneGroups ??= new ListEditor(serializedObject, "_sceneGroups", "Scene Groups");
			_sceneGroups.ShowIndex = true;
			SceneLoader.UpdateGroupNames();
		}

		override protected void CustomInspector(SceneGroup inst)
		{
			if (_sceneGroups == null) UpdateGroupsList();
			
			var loadOnStart = PropertyField("_loadOnStart").boolValue;
			if (loadOnStart)
			{
				EditorGUI.indentLevel++;
				var start = GetProperty("_startGroup");
				start.intValue = EditorGUILayout.Popup("Start Group", start.intValue, SceneLoader.GroupNames);
				EditorGUI.indentLevel--;
			}
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			_sceneGroups.DoLayoutList();
			if (EditorGUI.EndChangeCheck()) UpdateGroupsList();

			PropertyField("_activityIndicator");
			EditorGUILayout.Space();
			PropertyField("_onPreLoad");
			PropertyField("_onLoadComplete");
			PropertyField("_debugLogging");
		}
	}
}