using AdditiveSceneGroups;
using Events;

public class SceneLoadedEventInfo : EventInfoBase
{
    public SceneLoadedEventInfo(SceneSetScriptable sceneSet)
    {
        LoadedSceneSet = sceneSet;
    }

    public SceneSetScriptable LoadedSceneSet { get; }
}