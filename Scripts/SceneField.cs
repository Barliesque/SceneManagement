using System;
using UnityEngine;
using Object = UnityEngine.Object;


/// <summary>
/// SceneField allows a scene to be dragged into a field in the Inspector.
/// Found at:  https://answers.unity.com/questions/242794/inspector-field-for-scene-asset.html
/// See also: https://gist.github.com/JohannesMP/ec7d3f0bcf167dab3d0d3bb480e0e07b
/// </summary>

[Serializable]
public class SceneField
{
	[SerializeField] private Object sceneAsset;
	[SerializeField] private string sceneName = "";

	public string SceneName => sceneName;

	// makes it work with the existing Unity methods (LoadLevel/LoadScene)
	static public implicit operator string(SceneField sceneField)
	{
		return sceneField.SceneName;
	}
}
