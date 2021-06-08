using System.Collections;
using System.Collections.Generic;
using AdditiveSceneGroups;
using Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class SceneManagementWrapper : MonoBehaviour
{

    #region Fields

#pragma warning disable CS0649
    [SerializeField] private SceneBootstrapper sceneBootstrapper;
    [SerializeField] private List<SceneSetScriptable> scenes;
    private int currentSceneIndex = 0;
    
#if UNITY_EDITOR
    [Title("For Testing")]
#endif
    [SerializeField] private bool preventAutoLoadTitleSceen;
    [SerializeField] private SceneSetScriptable defaultTestingSceneSet;
#pragma warning restore CS0649
    #endregion
    
    #region Properties
    
    public float LoadingProgress => sceneBootstrapper.LoadProgress;
    public SceneSetScriptable CurrentSceneSetScriptable { get; private set; }

    /// <summary>
    /// Returns next configured scene.  If at the end of the scene array, returns the first scene in the array
    /// </summary>
    public SceneSetScriptable NextSceneSetScriptable
    {
        get
        {
            if (currentSceneIndex == scenes.Count - 1)
                return scenes[0];

            return scenes[currentSceneIndex + 1];
        }
    }

    public List<SceneSetScriptable> GameplayScenes => scenes;
    public bool IsLoadingInProgress { get; private set; }

    #region Singleton

    private static SceneManagementWrapper _instance;

    public static SceneManagementWrapper Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<SceneManagementWrapper>();

            return _instance;
        }
    }

    #endregion

    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        _instance = this;
        currentSceneIndex = 0;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    private void Start()
    {
        if (sceneBootstrapper == null) sceneBootstrapper = FindObjectOfType<SceneBootstrapper>();
#if UNITY_EDITOR
        EditorSceneManager.preventCrossSceneReferences = false;
        UnloadOpenEditorScenes();
        if (preventAutoLoadTitleSceen)
        {
            CurrentSceneSetScriptable = defaultTestingSceneSet;
            LoadSceneSet(CurrentSceneSetScriptable, CurrentSceneSetScriptable.loadOption);
            return;
        }
#endif
        LoadTitleScene();
    }
    
    #endregion

    #region Public Methods

    /// <summary>
    /// Assumes the title scene is the first in the scene array
    /// </summary>
    public void LoadTitleScene()
    {
        currentSceneIndex = 0;
        CurrentSceneSetScriptable = scenes[currentSceneIndex];
        LoadSceneSet(CurrentSceneSetScriptable, CurrentSceneSetScriptable.loadOption);
    }

#if UNITY_EDITOR
    [Button("Load Scene Set")]
#endif
    public void LoadSceneSet(SceneSetScriptable sceneSetScriptable, LoadOptions loadOptions)
    {
        if (sceneSetScriptable == null || !scenes.Contains(sceneSetScriptable))
        {
            Debug.LogWarning("Trying to load a null scene or scene that isnt added to the " +
                             "to the valid list of scenes on the SceneLoadWrapper");
            return;
        }
        CurrentSceneSetScriptable = sceneSetScriptable;
        
        IsLoadingInProgress = true;
        StartLoadingScreen();
        
        sceneBootstrapper.LoadSceneSet(sceneSetScriptable, loadOptions,
            () => FireOnSceneLoadedEvent(sceneSetScriptable));
        
        currentSceneIndex = scenes.IndexOf(sceneSetScriptable);
        CurrentSceneSetScriptable = sceneSetScriptable;
        IsLoadingInProgress = false;
    }

#if UNITY_EDITOR
    [Button("Load Next Level")]
#endif
    public void LoadNextSceneSet()
    {
        LoadSceneSet(NextSceneSetScriptable, NextSceneSetScriptable.loadOption);
    }

#if UNITY_EDITOR
    [Button("ReloadCurrentLevel")]
#endif
    public void ReloadCurrentSceneSet()
    {
        LoadSceneSet(CurrentSceneSetScriptable, LoadOptions.ReloadMatchingScenes);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
    
    private void FireOnSceneLoadedEvent(SceneSetScriptable sceneSet)
    {
        var eventInfo = new SceneLoadedEventInfo(sceneSet);
        EventManager.Instance.FireGlobalEvent(eventInfo);
    }
    
    #region Loading Screen Placeholder
    private void StartLoadingScreen()
    {
        //Debug.Log("To do:  Loading screen");
    }

    private void StopLoadingScreen()
    {
        //Debug.Log("To do:  Loading screen");
    }
    #endregion

    #region Editor Helpers
#if UNITY_EDITOR
    private void UnloadOpenEditorScenes()
    {
        if (SceneManager.sceneCount > 0)
            for (var i = 1; i < SceneManager.sceneCount; i++)
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
    }
#endif
    #endregion

}