using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public SceneData SceneData { get; set; }
    public SceneLoader SceneLoader { get; set;}

    public string currentCheckPointId;

    public bool keyPickedUp { get; set; }

    public ScorpionQuest ScorpionQuest { get; set; }

    public bool LeverPrison1 = true;
    public bool LeverPrison2 = false;
    public Dictionary<DoorName, bool> doorNameOpening = new Dictionary<DoorName, bool>();
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



    public void SetupOnLoad()
    /*
     When player changes rooms this will run
     */
    {
        InputManager.Instance.SaveAsync();
        ScorpionQuest.SetupQuestEvents();
    }

    private void Awake()
    {
        if (instance == null)
        {
            doorNameOpening = new Dictionary<DoorName, bool>();
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
        data.prison1 = LeverPrison1;
        data.prison2 = LeverPrison2;
        data.doorData = new List<DoorState>();
        foreach (var kvp in doorNameOpening)
        {
            data.doorData.Add(new DoorState { doorName = kvp.Key, isOpen = kvp.Value });
        }
    }

    public void Load(GameManagerData data)
    {
        keyPickedUp = data.keyPickedUp;
        LeverPrison1 = data.prison1;
        LeverPrison2 = data.prison2;
        foreach (var doorState in data.doorData)
        {
            doorNameOpening[doorState.doorName] = doorState.isOpen;
        }
    }
}

[System.Serializable]
public struct GameManagerData
{
    public bool keyPickedUp;
    public bool prison1;
    public bool prison2;
    public List<DoorState> doorData;
}

[System.Serializable]
public struct DoorState
{
    public DoorName doorName;
    public bool isOpen;
}