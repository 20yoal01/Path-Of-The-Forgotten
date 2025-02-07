using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public SceneFadeManager sceneFade;
    public Button LoadGameButton;
    bool SaveFileExists = false;

    private void Awake()
    {
        SaveFileExists = SaveSystem.SaveFileExists();
        LoadGameButton.interactable = SaveFileExists;
    }

    public void PlayGame()
    {
        SaveSystem.DeleteSave();
        StartCoroutine(FadeOutThenChangeScene());
    }

    public void LoadExistingGame()
    {
        if (SaveFileExists)
        {
            LoadGame();
            StartCoroutine(FadeOutThenChangeScene());
        }
    }
    public void LoadGame()
    {
        if (!_isLoading)
            LoadAsync();
    }

    private bool _isLoading;
    public async void LoadAsync()
    {
        _isLoading = true;
        await SaveSystem.LoadAync();
        _isLoading = false;
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

    private IEnumerator FadeOutThenChangeScene()
    {
        sceneFade.StartFadeOut();

        while (sceneFade.IsFadingOut)
        {
            yield return null;
        }
        SceneManager.LoadScene(2);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        sceneFade.StartFadeIn();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
