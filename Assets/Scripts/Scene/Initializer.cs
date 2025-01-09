using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Execute()
    {
        Debug.Log("Loaded by the Persist Object from the Initializer script");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Persistant Objects")));
    }
}
