using Barliesque.SceneManagement;
using UnityEditor;
using UnityEngine;

namespace Barliesque.InspectorTools.Editor
{

	[CustomPropertyDrawer(typeof(SceneLoader.SceneSelect))]
	public class SceneLoaderSelectDrawer : PropertyDrawerHelper
	{

		override public void CustomDrawer()
		{
			AdjustWidth(48f);
			var group = Field(180f, "group").objectReferenceValue as SceneGroup;

			var icon = StatusIcon(group);
			var pos = _position;
			pos.width = pos.height = 18f;
			GUI.Box(pos, icon);
			_position.x += 24f;
			
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

		private GUIContent StatusIcon(SceneGroup group)
		{
			int status = group ? 0 : 2; // 0 = green, 1 = yellow, 2 = red
			for (var i = 0; status < 2 && i < group.Scenes.Length; i++)
			{
				var item = group.Scenes[i];
				if (item is null) continue;

				var sceneIsSet = !string.IsNullOrEmpty(item.sceneField.ScenePath);
				if (!item.enabled && sceneIsSet)
				{
					// Scene is set, but disabled...
					status = Mathf.Max(status, 1);
				}

				if (!item.enabled) continue;
				if (!sceneIsSet)
				{
					// Enabled but not set?!
					status = 2;
					break;
				}

				var isInBuild = false;
				foreach (var buildScene in EditorBuildSettings.scenes)
				{
					if (buildScene.path != item.sceneField.ScenePath) continue;
					isInBuild = true;
					break;
				}

				if (isInBuild) continue;

				// Scene is not in the build!
				status = 2;
				break;
			}

			return status switch
			{
				0 => GreenLight(),
				1 => YellowLight(),
				2 => RedLight(),
				_ => new GUIContent()
			};
		}

		private GUIContent GreenLight()
		{
#if UNITY_6000_0_OR_NEWER
			return EditorGUIUtility.IconContent("d_greenLight");
#else
 			return EditorGUIUtility.IconContent("d_winbtn_mac_max");
#endif
		}

		private GUIContent YellowLight()
		{
#if UNITY_6000_0_OR_NEWER
			return EditorGUIUtility.IconContent("d_orangeLight");
#else
 			return EditorGUIUtility.IconContent("d_winbtn_mac_min");
#endif
		}

		private GUIContent RedLight()
		{
#if UNITY_6000_0_OR_NEWER
			return EditorGUIUtility.IconContent("d_redLight");
#else
 			return EditorGUIUtility.IconContent("d_winbtn_mac_close");
#endif
		}
	}
}