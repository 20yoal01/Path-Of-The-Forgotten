using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSetup : MonoBehaviour
{
    public GameObject bossUI;
    
    public Image bossImage;
    public TextMeshProUGUI titleText;
    public AudioClip bossIntroAudio;
    public AudioClip bossOST;

    public AudioSource audioSource;
    public AudioSource loopSource;

    private GameObject bossPrefab;
    private bool doOnce = false;
    BehaviorGraphAgent BGA;

    public Image EndbossImage;
    public TextMeshProUGUI EndtitleText;
    public SceneFadeManager sceneFade;

    private void Awake()
    {
        bossPrefab = GameObject.FindGameObjectWithTag("Boss");
        audioSource = GetComponent<AudioSource>();

        if (bossUI != null)
        {
            bossUI.gameObject.SetActive(false);
        }

        if (bossPrefab != null)
        {
            BGA = bossPrefab.GetComponent<BehaviorGraphAgent>();
            if (BGA != null)
                BGA.SetVariableValue("SpawnBoss", false);
        }

        GameObject.FindGameObjectWithTag("Fade")?.TryGetComponent(out sceneFade);

        if (sceneFade == null)
        {
            Debug.LogError("Could not find sceneFade");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && bossUI != null && bossPrefab != null && !doOnce)
        {
            BGA.SetVariableValue("SpawnBoss", true);
            StartCoroutine(PlayBossSequence());
            doOnce = true;
        }
    }

    private IEnumerator PlayBossSequence()
    {
        InputManager.Instance.DeactivatePlayerControls();
        yield return new WaitForSeconds(1f);
        FadeUIElements(0f, 1f, 0.2f);

        // Play the boss intro audio.
        MusicPlayer.Instance.toggleAudioSource(true);
        audioSource.clip = bossIntroAudio;
        audioSource.Play();
        audioSource.volume = 1f;

        // Wait until the audio finishes.
        yield return new WaitForSeconds(bossIntroAudio.length);

        // Fade out the image and text using DOTween.
        FadeUIElements(1f, 0f, 0.05f);

        audioSource.clip = bossOST;
        audioSource.Play();
        audioSource.volume = 0.1f;
        loopSource.PlayScheduled(AudioSettings.dspTime + audioSource.clip.length);
        InputManager.Instance.ActivatePlayerControls();
        BGA.SetVariableValue("EnableBoss", true);
        bossUI.gameObject.SetActive(true);
    }

    private void FadeUIElements(float startAlpha, float endAlpha, float duration)
    {
        // Fade the Image UI element using DOTween
        bossImage.DOFade(endAlpha, duration);

        // Fade the TextMeshPro UI element using DOTween
        titleText.DOFade(endAlpha, duration);
    }

    private void FadeEndUIElements(float startAlpha, float endAlpha, float duration)
    {
        EndbossImage.DOFade(endAlpha, duration);
        EndtitleText.DOFade(endAlpha, duration);
    }

    public void PlayBossEndSequence()
    {
        StartCoroutine(PlayBossEndSequenceCoroutine());
    }

    private IEnumerator PlayBossEndSequenceCoroutine()
    {
        InputManager.Instance.DeactivatePlayerControls();
        yield return new WaitForSeconds(1f);
        FadeEndUIElements(0f, 1f, 0.2f);
        yield return new WaitForSeconds(5);
        FadeEndUIElements(1f, 0f, 0.05f);
        InputManager.Instance.ActivatePlayerControls();
        StartCoroutine(FadeOutThenChangeScene());
    }


    private IEnumerator FadeOutThenChangeScene()
    {
        sceneFade.StartFadeOut();

        while (sceneFade.IsFadingOut)
        {
            yield return null;
        }
        SceneManager.LoadScene(0);
        InputManager.PlayerInput.SwitchCurrentActionMap("Player");
    }
}
