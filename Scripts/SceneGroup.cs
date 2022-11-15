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
			[SerializeField] private bool _disabled;
			public bool enabled => !_disabled;
			public SceneField sceneField;
			public Scene scene => SceneManager.GetSceneByPath(sceneField.SceneName);
			public bool alwaysReload;
			public bool setActive;

			override public string ToString()
			{
				return $"SceneGroup.Item: {sceneField.SceneName}";
			}

			public bool IsScene(string scenePath)
			{
				var a = scenePath;
				var b = $"Assets/{sceneField.SceneName}.unity";
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

		/*
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
		*/

		override public string ToString()
		{
			return $"SceneGroup[{name}]: {Scenes.Length} Items";
		}

	}
}