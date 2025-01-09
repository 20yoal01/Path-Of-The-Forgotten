using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    GameObject pauseMenu;
    
    public GameObject globalVolume;

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
}
