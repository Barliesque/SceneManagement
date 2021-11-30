using Barliesque.InspectorTools.Editor;
using UnityEditor;

namespace Wildlife.Escape.Editor
{
	[CustomEditor(typeof(SceneGroup))]
	public class SceneGroupEditor : EditorBase<SceneGroup>
	{
		// static private bool _showAdvanced;


		override protected void CustomInspector(SceneGroup inst)
		{
			EditorTools.ArrayEditor("Scenes", serializedObject.FindProperty("Scenes"));
			
			// if (EditorTools.BeginFoldout("Advanced", ref _showAdvanced))
			// {
			PropertyField("TetrahedralizeProbes");
			// }
			// EditorTools.EndFoldout(_showAdvanced);
		}
	}
}