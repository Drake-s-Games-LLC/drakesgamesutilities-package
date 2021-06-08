using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "AdditiveSceneGroups/Scene Set", fileName = "Scene Set")]
public class SceneSetScriptable : ScriptableObject
{
    [SerializeField] public SceneSet SceneSet;
    [SerializeField] public LoadOptions loadOption;
}

