using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initializer
{
    public static GameObject persistentObject;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Debug.Log("Loaded by the Persist Object from the Initializer script");

        persistentObject = Object.Instantiate(Resources.Load("Persistant Objects")) as GameObject;
        Object.DontDestroyOnLoad(persistentObject);
    }

}
