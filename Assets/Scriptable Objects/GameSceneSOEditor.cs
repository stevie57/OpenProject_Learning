using System;
using UnityEditor;
using UnityEngine;

public abstract partial class GameSceneSO : ScriptableObject, ISerializationCallbackReceiver
{
    public static Action<GameSceneSO> OnEnabled;

    private SceneAsset _prevSceneAsset;

    private void OnEnable()
    {
        _prevSceneAsset = null;
        PopulateScenePath();
        OnEnabled?.Invoke(this);
    }

    public void OnBeforeSerialize()
    {
        PopulateScenePath();
    }

    private void PopulateScenePath()
    {
        if(SceneAsset != null)
        {
            if(_prevSceneAsset != SceneAsset)
            {
                ScenePath = AssetDatabase.GetAssetPath(SceneAsset);
                _prevSceneAsset = SceneAsset;
            }
        }
        else
        {
            ScenePath = string.Empty;
        }
    }
    public void OnAfterDeserialize()
    { }
}
