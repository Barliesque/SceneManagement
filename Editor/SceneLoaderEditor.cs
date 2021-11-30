using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Wildlife.Escape.Editor
{
	
	[CustomEditor(typeof(SceneLoader))]
	public class SceneLoaderEditor : EditorBase<SceneGroup>
	{
		private SerializedProperty _groupsProp;
		private string[] _groups;
		
		override protected void OnEnabled(SceneGroup inst)
		{
			UpdateGroupsList();
		}

		private void UpdateGroupsList()
		{
			_groupsProp = GetProperty("_sceneGroups");
			_groups = new string[_groupsProp.arraySize];
			for (int i = 0; i < _groups.Length; i++)
			{
				_groups[i] = _groupsProp.GetArrayElementAtIndex(i).objectReferenceValue?.name ?? $"Group {i}";
			}
		}

		override protected void CustomInspector(SceneGroup inst)
		{
			if (_groups == null) UpdateGroupsList();
			var start = GetProperty("_startGroup");
			start.intValue = EditorGUILayout.Popup("Start Group", start.intValue, _groups);

			EditorGUI.BeginChangeCheck();
			EditorTools.ArrayEditor("Scene Groups", _groupsProp);
			if (EditorGUI.EndChangeCheck()) UpdateGroupsList();
			
			PropertyField("_onPreLoad");
			PropertyField("_onLoadComplete");
			PropertyField("_debugLogging");
		}
		
	}
}