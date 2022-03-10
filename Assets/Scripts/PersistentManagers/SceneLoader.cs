using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Persistent Manager Scene")]
    [SerializeField] private GameSceneSO _persistentManagersScene = default;

    [Header("Event Channels Listening")]
    //The level load event we are listening to
    [SerializeField] private LoadEventChannelSO _loadLevel = default;
    //The menu load event we are listening to    
    [SerializeField] private LoadEventChannelSO _loadMenu = default;

    [Header("Event Channels Broadcasting")]
    [SerializeField] private VoidEventChannelSO _OnSceneReady = default;

    private List<AsyncOperation> _scenesToLoadAsyncOperations = new List<AsyncOperation>();
    private List<Scene> _scenesToUnload = new List<Scene>();
    private GameSceneSO _activeScene; // The scene we want to set as active (for lighting/skybox)
    private List<GameSceneSO> _persistentScenes = new List<GameSceneSO>(); //Scenes to keep loaded when a load event is raised

    private void OnEnable()
    {
        if (_loadLevel != null)
            _loadLevel.OnLoadingRequested += LoadLevel;
        if (_loadMenu != null)
            _loadMenu.OnLoadingRequested += LoadMenu;
    }

    private void OnDisable()
    {
        if (_loadLevel != null)
            _loadLevel.OnLoadingRequested -= LoadLevel;
        if (_loadMenu != null)
            _loadMenu.OnLoadingRequested -= LoadMenu;
    }

    private void LoadLevel(GameSceneSO[] scenesToLoad, bool showLoadingScreen)
    {
        _persistentScenes.Add(_persistentManagersScene);
        AddScenesToUnload(_persistentScenes);
        LoadScenes(scenesToLoad, showLoadingScreen);
    }

    private void LoadMenu(GameSceneSO[] MenuToLoad, bool showLoadingScreen)
    {
        //When loading a menu, we only want to keep the persistent managers scene loaded
        _persistentScenes.Add(_persistentManagersScene);
        AddScenesToUnload(_persistentScenes);
        LoadScenes(MenuToLoad, showLoadingScreen);
    }

    private void AddScenesToUnload(List<GameSceneSO> persistentScenes)
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            string scenePath = scene.path;
            for(int j = 0; j < persistentScenes.Count; j++)
            {
                if(scenePath != persistentScenes[j].ScenePath)
                {
                    //Check if we reached the last persistent scenes check
                    if (j == persistentScenes.Count - 1)
                    {
                        _scenesToUnload.Add(scene);
                    }
                }
                else
                {
                    //We move the next scene check as soon as we find that the scene is one of the persistent scenes
                    break;
                }
            }
        }
    }
    private void LoadScenes(GameSceneSO[] scenesToLoad, bool showLoadingScreen)
    {
        //Take the first scene in the array as the scene we want to set active
        _activeScene = scenesToLoad[0];
        UnloadScenes();

        if (showLoadingScreen)
        {
            // implement a loading screen event
        }

        if(_scenesToLoadAsyncOperations.Count == 0)
        {
            for(int i = 0; i < scenesToLoad.Length; i++)
            {
                string currentScenePath = scenesToLoad[i].ScenePath;
                _scenesToLoadAsyncOperations.Add(SceneManager.LoadSceneAsync(currentScenePath));
            }
        }

        //Checks if any of the persistent scenes is not loaded yet and load it if unloaded
        //This is especially useful when we go from main menu to first location
        for (int i = 0; i < _persistentScenes.Count; ++i)
        {
            if (IsSceneLoaded(_persistentScenes[i].ScenePath) == false)
            {
                _scenesToLoadAsyncOperations.Add(SceneManager.LoadSceneAsync(_persistentScenes[i].ScenePath, LoadSceneMode.Additive));
            }
        }
        StartCoroutine(WaitForLoading(showLoadingScreen));

    }

    private IEnumerator WaitForLoading(bool showLoadingScreen)
    {
        bool loadingDone = false;
        while (!loadingDone)
        {
            for(int i = 0; i < _scenesToLoadAsyncOperations.Count; i++)
            {
                if (!_scenesToLoadAsyncOperations[i].isDone)
                {
                    break;
                }
                else
                {
                    loadingDone = true;
                    _scenesToLoadAsyncOperations.Clear();
                    _persistentScenes.Clear();
                }
            }
            yield return null;
        }

        SetActiveScene();
        if (showLoadingScreen)
        {
            //Raise event to disable loading screen 
            //_ToggleLoadingScreen.RaiseEvent(false);
        }
    }

    private void SetActiveScene()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_activeScene.ScenePath));
        // reconstruct light probe tetrahedrons to include the probes from newly-loaded scene
        LightProbes.TetrahedralizeAsync();
        // raise the event to inform that the scene is loaded and set active
        //_OnSceneReady.RaiseEvent();
    }

    private void UnloadScenes()
    {
       if(_scenesToUnload != null)
        {
            for(int i = 0; i < _scenesToUnload.Count; i++)
            {
                SceneManager.UnloadSceneAsync(_scenesToUnload[i]);
            }
            _scenesToUnload.Clear();
        }
    }

    private bool IsSceneLoaded(string ScenePath)
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if(scene.path == ScenePath)
            {
                return true;
            }
        }

        return false;
    }
}
