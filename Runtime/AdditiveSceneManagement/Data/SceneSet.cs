using System;

[Serializable]
public struct SceneSet 
{
    public string Name;
    public SceneData[] Scenes;

    public static SceneSet Empty => new SceneSet { Name = string.Empty, Scenes = new SceneData[0] };
}

    

