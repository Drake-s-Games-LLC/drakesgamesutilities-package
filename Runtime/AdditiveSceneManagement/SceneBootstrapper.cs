using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

namespace AdditiveSceneGroups {

    /// <summary>
    /// Automatically loads a set of scenes to the scene loader
    /// </summary>
    [DisallowMultipleComponent]
    public class SceneBootstrapper : MonoBehaviour {
        public SceneData[] PendingScenes => scenesToLoad;

#pragma warning disable CS0649
        [SerializeField] protected SceneLoader sceneLoader;
        [SerializeField, HideInInspector] protected SceneData[] scenesToLoad;
#pragma warning restore CS0649

        public float LoadProgress { get; private set; }

        protected enum LoadBehaviorType : byte { OverwriteLoadedScenes, AppendToLoadedScenes }


        /// <summary>
        /// Loads Scenes from scriptable using a loading screen
        /// </summary>
        /// <param name="sceneSetScriptable"></param>
        /// <param name="loadOption"></param>
        public void LoadSceneSet(SceneSetScriptable sceneSetScriptable, LoadOptions loadOption, Action callOnComplete = null)
        {
            LoadScenes(sceneSetScriptable, loadOption, true, callOnComplete);
        }

        private void LoadScenes(SceneSetScriptable sceneSetScriptable, LoadOptions loadOption, bool trackProgress = false, Action callOnComplete = null)
        {
            sceneLoader.LoadScenes(sceneSetScriptable.SceneSet.Scenes, loadOption);
            if (trackProgress)
            {
                StartCoroutine(TrackLoadProgress(callOnComplete));
            }
        }

        private IEnumerator TrackLoadProgress(Action callOnComplete = null)
        {
            LoadProgress = 0;
            
            while (LoadProgress < 1)
            {
                float tickProgress = 0;
                foreach (AsyncOperation op in sceneLoader.LoadingOperations)
                {
                    tickProgress += op.progress;
                }

                LoadProgress = ((tickProgress / (Mathf.Max(1,sceneLoader.LoadingOperations.Count) * 0.9f)));
                LoadProgress = LoadProgress + 0.01 > 1 ? 1 : LoadProgress;
                yield return null;
            }

            foreach (AsyncOperation op in sceneLoader.LoadingOperations)
            {
                op.allowSceneActivation = true;
            }
            if (callOnComplete != null) callOnComplete.Invoke();
        }
    }
}
