using System;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Barliesque.SceneManagement
{

	[CreateAssetMenu(fileName = "Scene Group", menuName = "Barliesque/Scene Group")]
	public class SceneGroup : ScriptableObject
	{

		public Item[] Scenes;
		
		[Tooltip("If Light Probes are in use in this group of scenes, then LightProbes.Tetrahedralize() must be called after scene loading has completed, to tell Unity to update the tetrahedral tessellation with data from all Light Probes for all currently loaded Scenes.")]
		public bool TetrahedralizeProbes = true;

		[Serializable]
		public class Item
		{
			[SerializeField] public SceneReference sceneField;
			[Tooltip("If disabled, the scene will not be loaded.")]
			[SerializeField] private bool _disabled;
			[Tooltip("If selected, the scene will be removed if it's in the outgoing group and reloaded if it's in the incoming group.")]
			[SerializeField] private bool _alwaysReload;
			[Tooltip("Will this be the main scene of the group?  If selected, lighting settings associated with this scene will be used.  This will also be the default target for newly instantiated GameObjects.")]
			[SerializeField] private bool _setActive;
			
			public bool enabled => !_disabled;
			public Scene scene => SceneManager.GetSceneByPath(sceneField.ScenePath);

			public bool SetActive => _setActive;

			public bool AlwaysReload => _alwaysReload;

			override public string ToString()
			{
				return $"SceneGroup.Item: {sceneField.ScenePath}";
			}

			public bool IsScene(string scenePath)
			{
				var a = scenePath;
				var b = $"Assets/{sceneField.ScenePath}.unity";
				var c = (a == b);
				return c;
			}
		}


		public Item Find(string scenePath)
		{
			foreach (var item in Scenes)
			{
				if (item.enabled && item.IsScene(scenePath))
				{
					return item;
				}
			}
			return null;
		}

		
		public bool Contains(Scene scene)
		{
			foreach (var item in Scenes)
			{
				if (item.enabled && item.scene == scene)
				{
					return true;
				}
			}
			return false;
		}
		

		override public string ToString()
		{
			return $"SceneGroup[{name}]: {Scenes.Length} Items";
		}

	}
}