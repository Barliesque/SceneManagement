using Barliesque.SceneManagement;
using UnityEditor;

namespace Barliesque.InspectorTools.Editor
{

	[CustomPropertyDrawer(typeof(SceneLoader.SceneSelect))]
	public class SceneLoaderSelectDrawer : PropertyDrawerHelper
	{

		override public void CustomDrawer()
		{
			AdjustWidth(48f);
			var group = Field(180f, "group").objectReferenceValue as SceneGroup;
			Label(16f, "ID:", "ID by which to reference this group.  Tick the box to enter a custom ID.");
			var customID = Field(20f, "customID").boolValue;
			_position.x -= 8f;
			if (customID)
			{
				Field(-1f, "id");
			}
			else
			{
				Label(-1f, group?.name ?? "<Unassigned>", null, EditorTools.StringField);
			}

		}
	}
}