using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

public class SaveSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerData;
        public SceneSaveData SceneSaveData;
        public RespawnData respawnData;
        public ScorpionQuestSaveData ScorpionQuestData;
        public GameManagerData gameManagerData;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    #region Save Async

    public static async Task SaveAsynchronously()
    {
        await SaveAsync();
    }

    private static async Task SaveAsync()
    {
        HandleSaveData();

        await File.WriteAllTextAsync(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    #endregion

    #region Load Async
    
    public static async Task LoadAync()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        _saveData = JsonUtility.FromJson<SaveData>(saveContent);

        await HandleLoadDataAsync();
    }

    private static async Task HandleLoadDataAsync()
    {
        await GameManager.Instance.SceneData.LoadAsync(_saveData.SceneSaveData);

        await GameManager.Instance.SceneData.WaitForSceneToBeFullyLoaded();

        GameManager.Instance.Load(_saveData.gameManagerData);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().Load(_saveData.PlayerData);

        GameManager.Instance.ScorpionQuest.Load(_saveData.ScorpionQuestData);

        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Respawn");

        foreach (var checkpoint in checkpoints)
        {
            Respawn respawn = checkpoint.GetComponent<Respawn>();
            Animator respawnAnimator = checkpoint.GetComponent<Animator>();
            if (respawn.id == _saveData.respawnData.id)
            {
                GameManager.Instance.currentCheckPointId = respawn.id;
                respawnAnimator.SetBool(AnimationString.ActiveCheckpoint, true);
            }
        }
    }

    #endregion

    private static void HandleSaveData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().Save(ref _saveData.PlayerData);

        GameManager.Instance.Save(ref _saveData.gameManagerData);
        GameManager.Instance.SceneData.Save(ref _saveData.SceneSaveData);
        Respawn.FindRespawnByID(GameManager.Instance.currentCheckPointId).Save(ref _saveData.respawnData);
        GameManager.Instance.ScorpionQuest.Save(ref _saveData.ScorpionQuestData);
    }

    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().Load(_saveData.PlayerData);

        GameManager.Instance.Load(_saveData.gameManagerData);
        GameManager.Instance.SceneData.Load(_saveData.SceneSaveData);
        GameManager.Instance.ScorpionQuest.Load(_saveData.ScorpionQuestData);

        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Respawn");

        foreach (var checkpoint in checkpoints)
        {
            Respawn respawn = checkpoint.GetComponent<Respawn>();
            Animator respawnAnimator = checkpoint.GetComponent<Animator>();
            if (respawn.id == _saveData.respawnData.id)
            {
                GameManager.Instance.currentCheckPointId = respawn.id;
                respawnAnimator.SetBool(AnimationString.ActiveCheckpoint, true);
            }
        }

    }
}
