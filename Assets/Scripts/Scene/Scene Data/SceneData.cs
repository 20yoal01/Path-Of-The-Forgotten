using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour
{
    public SceneDataSO Data;
    public bool IncludePersistantData = true;
    public bool IncludeDustEffect;
    public bool IncludeFogEffect;

    private GameObject persistentObject;

    private void Awake()
    {
        GameManager.Instance.SceneData = this;

        /*persistentObject = GameObject.FindGameObjectWithTag("Persistant");
        if (persistentObject != null && !IncludePersistantData)
        {
            Destroy(persistentObject);
        }
        else if(persistentObject == null)
        {
            persistentObject = Object.Instantiate(Resources.Load("Persistant Objects")) as GameObject;
            DontDestroyOnLoad(persistentObject);
        }*/
    }

    public void Save(ref SceneSaveData data)
    {
        data.SceneID = Data.UniqueName;
    }

    public void Load(SceneSaveData data)
    {
        string sceneIndex = data.SceneID;
        if (GameManager.Instance.isRespawning)
        {
            Respawn respawn = Respawn.FindRespawnByID(GameManager.Instance.currentCheckPointId);
            if (respawn == null)
            {
                sceneIndex = GameManager.Instance.currentCheckPointSceneIndex;
            }
            else
            {
                sceneIndex = respawn.sceneDataIndex;
            }

        }
        GameManager.Instance.SceneLoader.LoadSceneByIndex(sceneIndex);
    }

    public async Task LoadAsync(SceneSaveData data)
    {
        string sceneIndex = data.SceneID;
        if (GameManager.Instance.isRespawning)
        {
            Respawn respawn = Respawn.FindRespawnByID(GameManager.Instance.currentCheckPointId);
            if (GameManager.Instance.currentCheckPointSceneIndex != "")
            {
                sceneIndex = GameManager.Instance.currentCheckPointSceneIndex;
            }
            else if (respawn != null && respawn.sceneDataIndex != "")
            {
                sceneIndex = respawn.sceneDataIndex;
            }

        }
        await GameManager.Instance.SceneLoader.LoadSceneByIndexAsync(sceneIndex);
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