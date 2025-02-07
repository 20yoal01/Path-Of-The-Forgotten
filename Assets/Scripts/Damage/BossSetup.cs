using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BossSetup : MonoBehaviour
{
    public GameObject bossUI;
    
    public Image bossImage;
    public TextMeshProUGUI titleText;
    public AudioClip bossIntroAudio;
    public AudioSource audioSource;

    private GameObject bossPrefab;
    private bool doOnce = false;
    BehaviorGraphAgent BGA;

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

        // Wait until the audio finishes.
        yield return new WaitForSeconds(bossIntroAudio.length);

        // Fade out the image and text using DOTween.
        FadeUIElements(1f, 0f, 0.05f);
        MusicPlayer.Instance.toggleAudioSource(false);
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
}
