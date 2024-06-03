using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
#if UNITY_IPHONE
using UnityEngine.iOS;
#elif UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Barliesque.SceneManagement
{

	public class SceneLoader : MonoBehaviour
	{
		[Tooltip("If no other scene is loaded, this scene group will be loaded immediately.")]
		[SerializeField] private int _startGroup = 0;
		[SerializeField] private SceneGroup[] _sceneGroups;
		[SerializeField] private ActivityIndicator _activityIndicator = ActivityIndicator.White;
		[SerializeField] private bool _debugLogging;
		[SerializeField] private UnityEvent _onPreLoad;
		[SerializeField] private UnityEvent _onLoadComplete;


		private enum ActivityIndicator
		{
			DontShow = -1,
			WhiteLarge = 0,
			White = 1,
			Gray = 2,
		}
		
		
		static private SceneLoader _inst;

		static public int CurrentGroup { get; private set; } = int.MaxValue;
		static public int PreviousGroup { get; private set; } = -1;
		static public bool FirstLoad => (PreviousGroup < 0 || PreviousGroup >= _inst._sceneGroups.Length);

		static public string CurrentGroupName
		{
			get {
				if (CurrentGroup < 0 || CurrentGroup >= _inst._sceneGroups.Length)
				{
					return null;
				}
				return _inst._sceneGroups[CurrentGroup].name;
			}
		}

		static public string PreviousGroupName
		{
			get {
				if (PreviousGroup < 0 || PreviousGroup >= _inst._sceneGroups.Length)
				{
					return null;
				}
				return _inst._sceneGroups[PreviousGroup].name;
			}
		}


		static public bool IsBusyLoading { get; private set; } = false;

		static public CoroutineHandle WaitUntilLoaded() => Timing.RunCoroutine(_WaitUntilLoadCompleted());

		static private IEnumerator<float> _WaitUntilLoadCompleted()
		{
			do
			{
				yield return Timing.WaitForOneFrame;
			} while (IsBusyLoading);
		}
		
		/// <summary>
		/// Invoked when a scene load is initiated.  Note that listeners to this action are automatically removed after being invoked.
		/// </summary>
		static public event Action OnPreLoad;
		
		/// <summary>
		/// Invoked when a scene load completes.  Note that listeners to this action are automatically removed after being invoked.
		/// </summary>
		static public event Action OnLoadComplete;
		

		private string _thisScenePath;
		private Action _onLoaded;
		private List<SceneGroup.Item> _toBeLoaded;
		private List<string> _current = new();


		private void Awake()
		{
			if (_inst != null)
			{
				Destroy(gameObject);
				throw new Exception("SceneLoader already exists!");
			}
			_inst = this;

			// This scene may never be unloaded, since it contains the SceneLoader
			_thisScenePath = gameObject.scene.path;
		}

		private void Start()
		{
			Timing.RunCoroutine(_LoadStartGroup());
		}

		private IEnumerator<float> _LoadStartGroup()
		{
			_toBeLoaded = new List<SceneGroup.Item>();
			GetCurrentScenes();

			yield return Timing.WaitForOneFrame;
			
			if (_current.Count == 1) Load(_startGroup);
		}


		static public void Load(string groupName, Action onLoaded = null)
		{
			// Find the specified SceneGroup
			SceneGroup group = null;
			foreach (var g in _inst._sceneGroups)
			{
				if (g.name == groupName)
				{
					group = g;
					break;
				}
			}
			if (group == null)
			{
				throw new Exception($"Unknown group name: \"{groupName}\"");
			}

			Timing.RunCoroutine(_inst._LoadGroup(group, onLoaded));
		}

		static public void Load(SceneGroup group, Action onLoaded = null)
		{
			Timing.RunCoroutine(_inst._LoadGroup(group, onLoaded));
		}

		static public void Load(int groupIndex, Action onLoaded = null)
		{
			if (!_inst)
			{
				Debug.LogError("The SceneLoader is not instantiated and cannot load the next scene group.");
				return;
			}
			if (groupIndex >= _inst._sceneGroups.Length) throw new Exception($"Group index [{groupIndex}] not defined in the SceneLoader component!");
			Timing.RunCoroutine(_inst._LoadGroup(_inst._sceneGroups[groupIndex], onLoaded));
		}

		
		/// <summary>
		/// Ensure a SceneGroup is added to the loader.
		/// </summary>
		/// <param name="group">The SceneGroup to be added.</param>
		/// <returns>Returns the index of the SceneLoader, whether it was added or found to already be in the list.</returns>
		static public int AddSceneGroup(SceneGroup group)
		{
			var sceneList = _inst._sceneGroups;
			var found = Array.IndexOf(sceneList, group);
			if (found >= 0) return found;
			
			var newArray = new SceneGroup[sceneList.Length + 1];
			Array.Copy(_inst._sceneGroups, newArray, sceneList.Length);
			var last = sceneList.Length;
			newArray[sceneList.Length] = group;
			_inst._sceneGroups = newArray;
			return last;
		}


		private IEnumerator<float> _LoadGroup(SceneGroup group, Action onLoaded)
		{
			if (!group) throw new Exception($"SceneGroup is null!");
			if (IsBusyLoading) throw new Exception($"SceneLoader cannot load {group.name} because it is busy loading {CurrentGroupName}");
			
			_onLoaded = onLoaded;
			PreviousGroup = CurrentGroup;
			CurrentGroup = Array.IndexOf(_sceneGroups, group);
			
			if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Load Group:  {group.name} (Index {CurrentGroup})");
			if (CurrentGroup < 0) Debug.LogError($"<color=green>[SceneLoader]</color>  SceneGroup \"{group.name}\" needs to be added to the SceneLoader's list.");
			IsBusyLoading = true;

			if (_debugLogging && _onPreLoad != null) Debug.Log($"<color=green>[SceneLoader]</color>  OnPreLoad events");
			try {
				_onPreLoad?.Invoke();
			} catch (Exception e) { Debug.LogException(e); }

			yield return Timing.WaitForOneFrame;

			if (_debugLogging && OnPreLoad != null) Debug.Log($"<color=green>[SceneLoader]</color>  OnPreLoad events (scripted)");
			try {
				OnPreLoad?.Invoke();
			} catch (Exception e) { Debug.LogException(e); }

			yield return Timing.WaitForOneFrame;
			GetCurrentScenes();

			// Make a list of scenes that need to be loaded
			_toBeLoaded.Clear();
			for (int i = 0, len = group.Scenes.Length; i < len; i++)
			{
				if (group.Scenes[i].enabled)
				{
					_toBeLoaded.Add(group.Scenes[i]);
				}
			}
			if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Unload outgoing scene(s)");

			//  Unload scenes not in this group (and scenes that should always be reloaded)
			for (int i = _current.Count - 1; i >= 0; i--)
			{
				if (_current[i] == _thisScenePath) continue;

				// If the scene is already loaded and it doesn't have to always be reloaded, then it's a keeper
				var keeper = group.Find(_current[i]);
				if (keeper == null || keeper.AlwaysReload)
				{
					if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Unload: {_current[i]}");
					var scene = SceneManager.GetSceneByPath(_current[i]);
					SceneManager.UnloadSceneAsync(scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
				}
				else
				{
					if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Keep: {_current[i]}");

					// Already loaded?  Remove from the list of scenes to be loaded
					for (int j = 0; j < _toBeLoaded.Count; j++)
					{
						if (_toBeLoaded[j].IsScene(_current[i]))
						{
							_toBeLoaded.RemoveAt(j);
							break;
						}
					}
				}
			}

			ShowLoadingIndicator();
			// Wait a frame to allow the indicator to display
			yield return Timing.WaitForOneFrame;
			
			if (_toBeLoaded.Count > 0)
			{
				if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Load {_toBeLoaded.Count} incoming scene(s)");
				SceneManager.sceneLoaded += SceneLoaded;

				// Start loading new scenes
				for (int i = 0; i < _toBeLoaded.Count; i++)
				{
					var item = _toBeLoaded[i];
					if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Load {item.sceneField.ScenePath}");
					var op = SceneManager.LoadSceneAsync(item.sceneField, LoadSceneMode.Additive);
					// TODO:  Push op to a stack and monitor op.progress to return total progress (remap 0-0.9 to 0-1.0, because last 10% can't be monitored) 
					// TODO:  Use an optional Gauge<float> to send progress?
				}
			}
			else
			{
				if (_debugLogging) Debug.Log("<color=green>[SceneLoader]</color>  Nothing to load!");
				LoadCompleted();
			}
		}

		/// <summary>
		/// Get a list of the scenes currently open.  See: _current
		/// </summary>
		private void GetCurrentScenes()
		{
			_current.Clear();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				_current.Add(SceneManager.GetSceneAt(i).path);
			}
		}


		private void SceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  Loaded Scene: {scene.path}  IsValid: {scene.IsValid()}  IsLoaded: {scene.isLoaded}");
			var index = _toBeLoaded.FindIndex((s) => scene.path == s.sceneField.ScenePath);

			if (index >= 0)
			{
				if (_toBeLoaded[index].SetActive)
				{
					SceneManager.SetActiveScene(scene);
				}
				_toBeLoaded.RemoveAt(index);
			}

			if (_toBeLoaded.Count == 0)
			{
				// All done!
				Timing.RunCoroutine(_Complete());
			}
		}


		private void ShowLoadingIndicator()
		{
#if UNITY_IPHONE
			Handheld.SetActivityIndicatorStyle((ActivityIndicatorStyle)_activityIndicator);
#elif UNITY_ANDROID
			Handheld.SetActivityIndicatorStyle((AndroidActivityIndicatorStyle)_activityIndicator);
#endif
		}

		private float HideLoadingIndicator()
		{
#if UNITY_IPHONE || UNITY_ANDROID
			Handheld.StopActivityIndicator();
#endif
			return 0f;
		}


		private IEnumerator<float> _Complete()
		{
			if (_debugLogging) Debug.Log("<color=green>[SceneLoader]</color>  Finishing up...");
			SceneManager.sceneLoaded -= SceneLoaded;
			
			yield return Timing.WaitForSeconds(HideLoadingIndicator());

			// Wait up to one second to allow scene initialization to complete
			int oneSec = Application.targetFrameRate;
			float targetFPS = Application.targetFrameRate * 0.75f;
			for (int i = 0; i < oneSec; i++)
			{
				yield return Timing.WaitForOneFrame;
				var fps = 1f / Time.unscaledDeltaTime;
				if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  {i} sec, FPS: {fps}");
				if (fps >= targetFPS)
				{
					break;
				}
			};

			IsBusyLoading = false;
			LoadCompleted();
			if (_debugLogging) Debug.Log($"<color=green>[SceneLoader]</color>  LOAD COMPLETED: {CurrentGroupName}");
		}

		private void LoadCompleted()
		{
			if (CurrentGroup >= 0 && _sceneGroups[CurrentGroup].TetrahedralizeProbes)
			{
				LightProbes.Tetrahedralize();
			}
			
			try {
				_onLoadComplete?.Invoke();
			} catch (Exception e) { Debug.LogException(e); }

			try {
				_onLoaded?.Invoke();
			} catch (Exception e) { Debug.LogException(e); }
			_onLoaded = null;
			
			try {
				OnLoadComplete?.Invoke();
			} catch (Exception e) { Debug.LogException(e); }
			OnLoadComplete = null;
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				if (Input.GetKeyDown(KeyCode.A)) Load(0);
				else if (Input.GetKeyDown(KeyCode.B)) Load(1);
				else if (Input.GetKeyDown(KeyCode.C)) Load(2);
				else if (Input.GetKeyDown(KeyCode.D)) Load(3);
				else if (Input.GetKeyDown(KeyCode.E)) Load(4);
				else if (Input.GetKeyDown(KeyCode.F)) Load(5);
			}
		}
	}
}