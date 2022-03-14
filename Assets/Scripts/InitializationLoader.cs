using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{
    [Header("Persisten Managers Scene")]
    [SerializeField] private GameSceneSO _persistentManagersScene = default;

    [Header("Loading Settings")]
    [SerializeField] private GameSceneSO[] _scenesToLoad = default;
    [SerializeField] private bool _showLoadingScreen = default;

    [Header("Broadcast Channel")]
    [SerializeField] private LoadEventChannelSO _menuLoadChannel = default;
    [SerializeField] private LoadEventChannelSO _levelLoadChannel = default;

    void Start()
    {
        StartCoroutine(LoadSceneRoutine(_persistentManagersScene.ScenePath));
    }

    private IEnumerator LoadSceneRoutine(string scenePath)
    {
        AsyncOperation loadingSceneAsyncOp = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

        while (!loadingSceneAsyncOp.isDone)
        {
            yield return null;
        }

        _menuLoadChannel.RaiseEvent(_scenesToLoad, _showLoadingScreen);
        //_levelLoadChannel.RaiseEvent(_scenesToLoad, _showLoadingScreen);
    }
}
