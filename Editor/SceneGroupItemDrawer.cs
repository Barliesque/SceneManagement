using UnityEngine;
using Barliesque.InspectorTools.Editor;
using UnityEditor;


namespace Wildlife.Escape.Editor
{

	[CustomPropertyDrawer(typeof(SceneGroup.Item))]
	public class SceneGroupItemDrawer : PropertyDrawerHelper
	{
		override public void CustomDrawer()
		{
			Field(50f, "Disable", 20f, "_disabled");
			var disabled = Property.FindPropertyRelative("_disabled").boolValue;
			GUI.enabled = !disabled;

			Field(100f, "Always Reload?", 20f, "alwaysReload");
			Field(70f, "Set Active", 20f, "setActive");
			NextLine();
			Field(290f, "sceneField");

			GUI.enabled = true;
		}


	}
}