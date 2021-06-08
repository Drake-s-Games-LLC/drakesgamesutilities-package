using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public enum LoadOptions : byte {
    ClearExistingScenes = 1 << 0,       // Every loaded scene in the pool is unloaded
    ReloadMatchingScenes = 1 << 1,  // For every matching scene in the new set, unload and reload the scene
    KeepMatchingScenes = 1 << 2   // Every matching scene is preserved
}

/// <summary>
/// A high-level simple scene loading manager that loads batches of scenes in one go
/// </summary>
[CreateAssetMenu(menuName = "AdditiveSceneGroups/Scene Loader", fileName = "Scene Loader")]
public class SceneLoader : ScriptableObject {

    public SceneData[] ActivePool => pool;
    public string[] ActiveScenes => FetchSceneNames();

    public List<AsyncOperation> LoadingOperations { get; private set; }

#pragma warning disable 649
    [Header("Editor Options")]
    [SerializeField, Tooltip(@"Prevents the loader from preloading currently-loaded scenes into the pool. 
    Enable this to simulate build loads from the first scene.")]
    private bool ignoreInitialScenes;
#pragma warning restore 649

    private SceneData[] pool;

    private const LoadOptions ForceUnloadOptions = LoadOptions.ClearExistingScenes | LoadOptions.ReloadMatchingScenes;

    private void OnEnable() {
        pool = new SceneData[0];
        LoadingOperations = new List<AsyncOperation>();

        if (!ignoreInitialScenes) {
            CheckForStartup();
        }
    }

    public void LoadScenes(SceneData[] scenes, LoadOptions options) {
        if (scenes.Length < 1) {
            throw new InvalidOperationException("Cannot load 0 scenes!");
        }
#if UNITY_EDITOR
        if (!ignoreInitialScenes) {
            CheckForStartup();
        }
#endif

        // Compare the scene and the current set to find matches
        CompareSceneSets(pool, scenes, out var currentMatches, out var pendingMatches);
        var shouldReloadIfMatch = (options & ForceUnloadOptions) != 0;
        UnloadScenes(pool, currentMatches, shouldReloadIfMatch, () => {
            LoadPendingScenes(scenes, pendingMatches, shouldReloadIfMatch, () => {
                // Rebuild the scene list
                pool = new SceneData[pendingMatches.Length];
                scenes.CopyTo(pool, 0);
            });
        });


    }

    public void ClearAndLoadScenes(SceneData[] scenes) {
        var matches = new bool[scenes.Length];
        for (int i = 0; i < matches.Length; i++) {
            matches[i] = true;
        }

        var matchesinCurrentPool = new bool[pool.Length];
        for (int i = 0; i < matchesinCurrentPool.Length; i++) {
            matchesinCurrentPool[i] = true;
        }

        UnloadScenes(pool, matchesinCurrentPool, true, () => {
            LoadPendingScenes(scenes, matches, true, () => {
                pool = new SceneData[scenes.Length];
                scenes.CopyTo(pool, 0);
            });
        });
    }

    private void CompareSceneSets(SceneData[] current, SceneData[] pending, out bool[] currentMatches, out bool[] pendingMatches) {
        currentMatches = new bool[current.Length];
        pendingMatches = new bool[pending.Length];

        for (var i = 0; i < current.Length; ++i) {
            currentMatches[i] = FindIndex(current[i].Name, pending) > -1;
        }

        for (var i = 0; i < pending.Length; ++i) {
            pendingMatches[i] = FindIndex(pending[i].Name, current) > -1;
        }
    }

    private string[] FetchSceneNames() {
        var scenes = new string[pool.Length];

        for (var i = 0; i < scenes.Length; ++i) {
            scenes[i] = pool[i].Name;
        }

        return scenes;
    }

    private int FindIndex(string name, SceneData[] scenes) {
        for (var i = 0; i < scenes.Length; ++i) {
            if (scenes[i].Name == name) {
                return i;
            }
        }

        return -1;
    }

    private void LoadPendingScenes(SceneData[] scenes, bool[] matches, bool shouldLoadIfMatch, Action onComplete) {
        if (scenes.Length < 1) {
            onComplete?.Invoke();
            return;
        }

        var total = 0;
        var loadCount = 0;

        for (var i = 0; i < scenes.Length; ++i) {
            if (!matches[i] || shouldLoadIfMatch) {
                total++;

                var scene = scenes[i];
                var op = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);

                LoadingOperations.Add(op);
                op.allowSceneActivation = false;

                op.completed += c => {
                    if (scene.IsMainScene) {
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.Name));
                    }

                    if (++loadCount >= total) {
                        onComplete?.Invoke();
                    }

                    LoadingOperations.Remove(op);
                };
            }
        }
        if (total < 1)
        {
            onComplete?.Invoke();
        }
    }

    private void UnloadScenes(SceneData[] scenes, bool[] matches, bool unloadMatches, Action onComplete) {
        if (scenes.Length < 1) {
            onComplete?.Invoke();
            return;
        }

        var total = 0;
        var unloadCount = 0;
        for (var i = 0; i < scenes.Length; ++i) {
            if (!matches[i] || unloadMatches) {
                total++;

                var op = SceneManager.UnloadSceneAsync(scenes[i].Name);
                
                op.completed += c => {
                    if (++unloadCount >= total) {
                        onComplete();
                    }
                };
            }
        }

        if (total == 0) {
            onComplete();
        }
    }


    private void CheckForStartup() {
        if (pool.Length == 0) {
            pool = new SceneData[SceneManager.sceneCount];
            for (var i = 0; i < pool.Length; ++i) {
                var scene = SceneManager.GetSceneAt(i);
                pool[i] = new SceneData {
                    Name = scene.name,
                    Path = scene.path,
                    IsMainScene = !scene.isSubScene
                };
            }

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying) {
                // Insert a new scene to serve as a "global" scene
                SceneManager.CreateScene("Simulated Global Scene");
            }
#endif
        }
    }
}

