using HeneGames.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Eleonore : MonoBehaviour
{
    public UnityEvent questTrigger;
    public DialogueManager eleonoreDialogueManager;
    public UnityEvent deactivateQuest;
    public UnityEvent defaultLine;

    public GameObject questReward;
    public Transform spawnPoint;

    public void TriggerQuestLine()
    {
        DialogueCharacter character = eleonoreDialogueManager.sentences[0].dialogueCharacter;

        questTrigger.Invoke();
        eleonoreDialogueManager.sentences.Clear();

        NPC_Centence defaultLine = new NPC_Centence();
        defaultLine.dialogueCharacter = character;
        defaultLine.sentence = "Hast thou not yet returned with victory? The venomous scourge waits not for idle words. Go, and let thy deeds be thy answer.";

        eleonoreDialogueManager.sentences.Add(defaultLine);
    }

    public void TriggerQuestComplete()
    {
        DialogueCharacter character = eleonoreDialogueManager.sentences[0].dialogueCharacter;
        eleonoreDialogueManager.sentences.Clear();
        NPC_Centence defaultLine = new NPC_Centence();
        defaultLine.dialogueCharacter = character;
        defaultLine.sentence = "Thy task is done, and the venomous plague is no more. Thou hast proven thyself worthy of far more than mere words of thanks. As promised, thy reward awaits—take it, and let its power guide thee on thy journey";
        defaultLine.sentenceEvent = deactivateQuest;

        eleonoreDialogueManager.sentences.Add(defaultLine);
        
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        if (!GameManager.Instance.keyPickedUp)
            Instantiate(questReward, spawnPosition, Quaternion.identity);
    }

    public void QuestCompleteDefaultLine()
    {
        DialogueCharacter character = eleonoreDialogueManager.sentences[0].dialogueCharacter;
        eleonoreDialogueManager.sentences.Clear();
        NPC_Centence defaultLine = new NPC_Centence();
        defaultLine.dialogueCharacter = character;
        defaultLine.sentence = "The land breathes easier thanks to thee. But linger not, for greater trials surely lie ahead.";
        eleonoreDialogueManager.sentences.Add(defaultLine);
    }
}
