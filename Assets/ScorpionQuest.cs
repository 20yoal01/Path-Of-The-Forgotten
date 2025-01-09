using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ScorpionQuest : MonoBehaviour
{
    public TMP_Text scorpionsStatusText;
    public bool questTriggered;
    public int scorpionsKilled;
    public int scorpionsRequired;
    public bool questComplete;

    public UnityEvent questCompleted;

    private void Awake()
    {
        GameManager.Instance.ScorpionQuest = this;
        gameObject.SetActive(false);
        scorpionsStatusText.text = $"0 / {scorpionsRequired}";
    }

    public void ActivateQuest()
    {
        questTriggered = true;
        gameObject.SetActive(true);
    }

    public void DeactivateQuest()
    {
        questTriggered = false;
        gameObject.SetActive(false);
    }

    public void OnScorpionKilled()
    {
        if (questTriggered)
        {
            scorpionsKilled++;
            scorpionsStatusText.text = $"{scorpionsKilled} / {scorpionsRequired}";

            if (scorpionsKilled >= scorpionsRequired)
            {
                questCompleted.Invoke();
                questComplete = true;
            }
        }
    }

    public void Load(ScorpionQuestSaveData data)
    {
        scorpionsKilled = data.scorpionsKilled;
        questTriggered = data.questTriggered;
        questComplete = data.questComplete;

        scorpionsStatusText.text = $"{scorpionsKilled} / {scorpionsRequired}";

        if (questComplete)
        {
            questCompleted.Invoke();
            DeactivateQuest();
        }
        else if (questTriggered)
        {
            ActivateQuest();
        }
    }

    public void Save(ref ScorpionQuestSaveData data)
    {
        data.scorpionsKilled = scorpionsKilled;
        data.questTriggered = questTriggered;
        data.questComplete = questComplete;
    }
}

[System.Serializable]
public struct ScorpionQuestSaveData
{
    public int scorpionsKilled;
    public bool questTriggered;
    public bool questComplete;
}