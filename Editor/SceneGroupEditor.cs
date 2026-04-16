using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Barliesque.SceneManagement.Editor
{
	
	[CustomEditor(typeof(SceneGroup))]
	public class SceneGroupEditor : EditorBase<SceneGroup>
	{
		
		private ListEditor _scenes;
		
		override protected void OnEnabled(SceneGroup inst)
		{
			_scenes = new ListEditor(serializedObject, "Scenes", "Scenes", 2);
		}

		override protected void CustomInspector(SceneGroup inst)
		{
			_scenes.DoLayoutList();

			var active = 0;
			for (int i = 0; i < _scenes.Count; i++)
			{
				var item = _scenes.GetElement(i);
				if (item != null
				    && !item.FindPropertyRelative("_disabled").boolValue
				    && item.FindPropertyRelative("_setActive").boolValue) ++active;
			}

			if (active == 0) EditorTools.HelpBox("One scene in the group should be set as the active scene.", MessageType.Warning);
			if (active > 1) EditorTools.HelpBox("Only one scene can be set active.", MessageType.Error);
			
			PropertyField("TetrahedralizeProbes");
		}
	}
}