using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using System;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI cutsceneText;
    public TextMeshProUGUI userInputText;

    public GameObject mainMenuUI;
    public GameObject optionsUI;
    public GameObject cutsceneUI;
    public static CutsceneManager Instance { get; private set; }
    public List<string> DialogueTexts;

    private bool isWaitingForContinue = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    Action onCutsceneComplete;

    public void StartCutscene(Action onCutsceneComplete = null)
    {
        mainMenuUI.SetActive(false);
        optionsUI.SetActive(false);
        cutsceneUI.SetActive(true);

        cutsceneText.gameObject.SetActive(true);
        cutsceneText.DOFade(0f, 0f);

        userInputText.gameObject.SetActive(false);
        this.onCutsceneComplete = onCutsceneComplete;
        IntroMusicPlayer.Instance.toggleAudioSource(true);
        PlayCutscene(0, onCutsceneComplete);
    }

    private void PlayCutscene(int index, Action onCutsceneComplete)
    {
        if (index >= DialogueTexts.Count)
        {
            cutsceneText.gameObject.SetActive(false);
            userInputText.gameObject.SetActive(false);

            onCutsceneComplete?.Invoke();
            return;
        }

        if (isWaitingForContinue)
            return;

        cutsceneText.text = DialogueTexts[index];

        float fadeLength = 1f;
        float delayBeforeFade = 0f;

        if (index == 0)
        {
            fadeLength = 3f;
            delayBeforeFade = 2f; 
        }

        cutsceneText.DOFade(1f, fadeLength).SetDelay(delayBeforeFade).OnComplete(() =>
        {
            userInputText.gameObject.SetActive(true);
            userInputText.DOFade(1f, 0f).OnComplete(() =>
            {
                isWaitingForContinue = true; // Wait for player input
            });
        });
    }

    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.performed && isWaitingForContinue)
        {
            isWaitingForContinue = false; // Disable input waiting

            userInputText.gameObject.SetActive(false);

            // Fade out the current dialogue text and proceed to the next
            cutsceneText.DOFade(0f, 1f).OnComplete(() =>
            {
                PlayCutscene(DialogueTexts.IndexOf(cutsceneText.text) + 1, onCutsceneComplete);
            });
        }
    }
}
