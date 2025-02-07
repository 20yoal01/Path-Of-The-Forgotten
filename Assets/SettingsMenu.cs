using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void MuteVolume (bool mute)
    {
        float volumeValue = mute ? -80 : 0;
        audioMixer.SetFloat("Volume", volumeValue);
    }

    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
