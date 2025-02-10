using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public SceneFadeManager sceneFade;
    public GameObject LoadGameButton;

    bool SaveFileExists = false;
    public GameState gameState;
     
    private void Awake()
    {
        SaveFileExists = SaveSystem.SaveFileExists();

        Button LoadGameButtonUI;
        EventTrigger LoadGameButtonES;

        LoadGameButton.TryGetComponent(out LoadGameButtonUI);
        LoadGameButton.TryGetComponent(out LoadGameButtonES);

        if (LoadGameButtonUI != null && LoadGameButtonES != null)
        {
            LoadGameButtonUI.interactable = SaveFileExists;
            LoadGameButtonES.enabled = SaveFileExists;
        }
    }

    public void PlayGame()
    {
        CutsceneManager.Instance.StartCutscene(() =>
        {
            SaveSystem.DeleteSave();
            Initializer.ResetState = true;
            Initializer.gameState = gameState;
            StartCoroutine(FadeOutThenChangeScene());
        });
    }

    public void LoadExistingGame()
    {
        if (SaveFileExists)
        {
            StartCoroutine(LoadGameWithDelay());
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

    private IEnumerator LoadGameWithDelay()
    {
        yield return new WaitForSeconds(1f);

        LoadGame();
        StartCoroutine(FadeOutThenChangeScene());
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
