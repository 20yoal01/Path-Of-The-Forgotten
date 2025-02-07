using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    
    public GameObject globalVolume;
    public SceneFadeManager sceneFade;

    public void Pause()
    {
        float cutOffFrequency = 2000f;

        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        InputManager.PlayerInput.SwitchCurrentActionMap("UI");
        MusicPlayer.Instance.controlMuffle(cutOffFrequency);

        BlurGame(true);
    }

    public void Resume()
    {
        float cutOffFrequency = 20000f;

        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        InputManager.PlayerInput.SwitchCurrentActionMap("Player");
        MusicPlayer.Instance.controlMuffle(cutOffFrequency);

        BlurGame(false);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(FadeOutThenChangeScene());
        SceneManager.activeSceneChanged += sceneUnloaded;
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

    private void sceneUnloaded(Scene oldScene, Scene newScene )
    {
        if (pauseMenu != null && pauseMenu.activeSelf)
            Resume();
    }

    public void BlurGame(bool shouldBlur)
    {
        Volume globalVolumeController = globalVolume.GetComponent<Volume>();
        DepthOfField depthOfField;

        if (globalVolumeController != null)
        {
            if (globalVolumeController.profile.TryGet(out depthOfField))
            {
                depthOfField.active = shouldBlur;
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
