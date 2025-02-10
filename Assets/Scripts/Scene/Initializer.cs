using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer
{
    public static GameObject persistentObject;
    public static bool ResetState;
    public static GameState gameState;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Debug.Log("Loaded by the Persist Object from the Initializer script");

        persistentObject = Object.Instantiate(Resources.Load("Persistant Objects")) as GameObject;
        Object.DontDestroyOnLoad(persistentObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }


    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckIsNeedEnabling(scene);
    }

    private static void OnSceneUnloaded(Scene scene) // Fixed method name
    {
        CheckIsNeedEnabling(scene);
    }

    private static void CheckIsNeedEnabling(Scene scene)
    {
        Debug.Log($"Scene Loaded: {scene.name}");

        // Find all SceneData objects in the scene
        SceneData[] sceneDataObjects = Object.FindObjectsByType<SceneData>(FindObjectsSortMode.None);

        bool shouldDisable = false;
        foreach (SceneData data in sceneDataObjects)
        {
            if (!data.IncludePersistantData)
            {
                Debug.Log("Disabling Persistence Object because SceneData has dontLoadPersistance set to false.");
                shouldDisable = true;
                break; // No need to keep checking after we find one
            }
        }

        bool willPlayMusic = false;

        // Ensure persistentObject is not null before setting active state
        if (persistentObject != null)
        {
            if (!persistentObject.activeSelf && !shouldDisable)
            {
                willPlayMusic = true;
            }
            persistentObject.SetActive(!shouldDisable);
            GameManager gameManager = Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null && ResetState)
            {
                gameManager.StartNewGame(gameState);
                ResetState = false;
            }

            if (willPlayMusic)
            {
                MusicPlayer.Instance.RestartMusic();
                willPlayMusic = false;
            }
        }
    }

}
