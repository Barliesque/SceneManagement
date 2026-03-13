A system to manage the loading and unloading of scenes and groups of scenes in Unity.

NOTE:  *This package requires **[More Effective Coroutines (MEC)](https://assetstore.unity.com/packages/tools/animation/more-effective-coroutines-free-54975)**


___Getting Started___
The first scene Unity loads should have the `SceneLoader` instance; this scene will never be unloaded, so it shouldn't have much else.  Create `SceneGroup` objects (**Create » Barliesque » Scene Group**) and specify one or more scenes that will need to be loaded.  Drag your `SceneGroup` objects into the `SceneLoader` component inspector.

By default, the first Scene Group in the list will automatically load in when the app starts.  You can disable this behavior, or select a different scene group with the "Start Group" property in the `SceneLoader`.

`SceneGroups` are referenced using their name as an identifier.  Override this with a custom ID, by clicking the ID checkbox and entering a new identifier.


___SceneGroup Options___
- **Disable** - Disable a scene from being loaded, without having to remove it from the `SceneGroup`.
- **Set Active** - One scene in the group should always be set as the active scene.
- **Always Reload** - If, for instance, a scene is part of two groups, you can decide if that scene should be reloaded or simply left alone when loading between these two groups.
- **Tetrahedralize Probes** - This is a necessary part of Unity's lighting system initialization whenever lights are loaded in with a scene.

___Scripting API___
The `SceneLoader` is a singleton class with a straight-forward static API--a direct link to the instance is not necessary.  For instance, to load a `SceneGroup`, call:

`SceneLoader.Load("MainMenu);`

Any scenes that had already been loaded (and that are not part of the "MainMenu" group) will first be unloaded, and then the scenes in the "MainMenu" group will be loaded.  Again, the start scene, containing the `SceneLoader` itself is never unloaded.

