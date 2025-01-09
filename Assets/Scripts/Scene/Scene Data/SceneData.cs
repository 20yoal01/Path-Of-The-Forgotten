using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour
{
    public SceneDataSO Data;

    private void Awake()
    {
        GameManager.Instance.SceneData = this;
    }

    public void Save(ref SceneSaveData data)
    {
        data.SceneID = Data.UniqueName;
    }

    public void Load(SceneSaveData data)
    {
        GameManager.Instance.SceneLoader.LoadSceneByIndex(data.SceneID);
    }

    public async Task LoadAsync(SceneSaveData data)
    {
        await GameManager.Instance.SceneLoader.LoadSceneByIndexAsync(data.SceneID);
    }

    public Task WaitForSceneToBeFullyLoaded()
    {
        TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

        UnityEngine.Events.UnityAction<Scene, LoadSceneMode> sceneLoaderHandler = null;

        sceneLoaderHandler = (scene, modee) =>
        {
            taskCompletion.SetResult(true);
            SceneManager.sceneLoaded -= sceneLoaderHandler;
        };

        SceneManager.sceneLoaded += sceneLoaderHandler;

        return taskCompletion.Task;
    }
}

[System.Serializable]
public struct SceneSaveData
{
    public string SceneID;
}