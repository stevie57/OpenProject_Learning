using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationLoader : MonoBehaviour
{
    [Header("Persisten Managers Scene")]
    [SerializeField] private GameSceneSO _persistentManagersScene = default;

    [Header("Loading Settings")]
    [SerializeField] private GameSceneSO[] _menuToLoad = default;
    [SerializeField] private bool _showLoadingScreen = default;

    [Header("Broadcast Channel")]
    [SerializeField] private LoadEventChannelSO _MenuLoadChannel = default; 


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

        _MenuLoadChannel.RaiseEvent(_menuToLoad, _showLoadingScreen);
    }
}
