using UnityEngine;
using Barliesque.InspectorTools.Editor;
using UnityEditor;


namespace Barliesque.SceneManagement.Editor
{

	[CustomPropertyDrawer(typeof(SceneGroup.Item))]
	public class SceneGroupItemDrawer : PropertyDrawerHelper
	{
		override public void CustomDrawer()
		{
			Space(10f);
			Field(50f, "Disable", 34f, "_disabled");
			var disabled = Property.FindPropertyRelative("_disabled").boolValue;
			GUI.enabled = !disabled;

			Field(70f, "Set Active", 34f, "_setActive");
			Field(90f, "Always Reload", 34f, "_alwaysReload");
			NextLine();
			Space(10f);
			Field(310f, "sceneField");

			GUI.enabled = true;
		}


	}
}