using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Load Event Channel")]
public class LoadEventChannelSO : EventChannelBaseSO
{
    public UnityAction<GameSceneSO[], bool> OnLoadingRequested;

    public void RaiseEvent(GameSceneSO[] scenesToLoad, bool showLoadingScreen)
    {
        if(OnLoadingRequested != null)
        {
            OnLoadingRequested(scenesToLoad, showLoadingScreen);
        }
        else
        {
            Debug.LogWarning("A Scene loading was requested, but nobody picked it up." +
                            "Check why there is no SceneLoader already present, " +
                            "and make sure it's listening on this Load Event channel.");

        }
    }
}
