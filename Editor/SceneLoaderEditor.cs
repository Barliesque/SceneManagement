using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Barliesque.SceneManagement.Editor
{
	[CustomEditor(typeof(SceneLoader))]
	public class SceneLoaderEditor : EditorBase<SceneGroup>
	{
		private string[] _groups;

		private ListEditor _sceneGroups;

		override protected void OnEnabled(SceneGroup inst)
		{
			UpdateGroupsList();
		}

		private void UpdateGroupsList()
		{
			_sceneGroups ??= new ListEditor(serializedObject, "_sceneGroups", "Scene Groups");

			_groups = new string[_sceneGroups.Count];
			for (int i = 0; i < _groups.Length; i++)
			{
				var group = _sceneGroups.GetElement(i).objectReferenceValue;
				_groups[i] = group ? group.name : $"Group {i}";
			}
		}

		override protected void CustomInspector(SceneGroup inst)
		{
			if (_groups == null || _sceneGroups == null) UpdateGroupsList();
			
			var start = GetProperty("_startGroup");
			start.intValue = EditorGUILayout.Popup("Start Group", start.intValue, _groups);
			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			_sceneGroups.DoLayoutList();
			if (EditorGUI.EndChangeCheck()) UpdateGroupsList();

			PropertyField("_activityIndicator");
			PropertyField("_onPreLoad");
			PropertyField("_onLoadComplete");
			PropertyField("_debugLogging");
		}
	}
}