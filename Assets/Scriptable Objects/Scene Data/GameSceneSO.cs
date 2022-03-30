using UnityEngine;

public abstract partial class GameSceneSO : ScriptableObject
{
    [Header("Information")]
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
#endif

    public string ScenePath;
    [TextArea] public string ShortDescription;
}
