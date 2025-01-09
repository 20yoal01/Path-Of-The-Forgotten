using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public SceneData SceneData { get; set; }
    public SceneLoader SceneLoader { get; set;}

    public Respawn currentCheckPoint;

    public bool keyPickedUp { get; set; }

    public ScorpionQuest ScorpionQuest { get; set; }

    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return null;
            }

            if (instance == null)
            {
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Save(ref GameManagerData data)
    {
        data.keyPickedUp = keyPickedUp;
    }

    public void Load(GameManagerData data)
    {
        keyPickedUp = data.keyPickedUp;
    }
}

[System.Serializable]
public struct GameManagerData
{
    public bool keyPickedUp;
}