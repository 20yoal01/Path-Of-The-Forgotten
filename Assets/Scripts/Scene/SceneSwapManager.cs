using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager instance;

    private DoorTriggerInteraction.DoorToSpawnAt _doorToSpawnTo;

    private static bool _loadFromDoor;

    private GameObject _player;
    private Collider2D _playerColl;
    private Transform doorLocation;
    private Vector3 _playerSpawnPosition;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerColl = _player.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void SwapSceneFromDoorUse(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt)
    {
        _loadFromDoor = true;
        instance.StartCoroutine(instance.FadeOutThenChangeScene(myScene, doorToSpawnAt));
    }

    private IEnumerator FadeOutThenChangeScene(SceneField myScene, DoorTriggerInteraction.DoorToSpawnAt doorToSpawnAt = DoorTriggerInteraction.DoorToSpawnAt.None)
    {
        InputManager.Instance.DeactivatePlayerControls();
        SceneFadeManager.instance.StartFadeOut();

        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }

        _doorToSpawnTo = doorToSpawnAt;
        SceneManager.LoadScene(myScene);
    }

    private IEnumerator ActivatePlayerControlsAfterFadeIn()
    {
        while (SceneFadeManager.instance.IsFadingIn)
        {
            yield return null;
        }

        InputManager.Instance.ActivatePlayerControls();
        GameManager.Instance.SetupOnLoad();
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneFadeManager.instance.StartFadeIn();

        if (_loadFromDoor)
        {
            StartCoroutine(ActivatePlayerControlsAfterFadeIn());
            FindDoor(_doorToSpawnTo);
            _player.transform.position = _playerSpawnPosition;
            _loadFromDoor = false;
        }
    }

    private void FindDoor(DoorTriggerInteraction.DoorToSpawnAt doorSpawnNumber)
    {
        DoorTriggerInteraction[] doors = FindObjectsByType<DoorTriggerInteraction>(FindObjectsSortMode.InstanceID);

        for (int i = 0; i < doors.Length; i++)
        {
            if (doors[i].CurrentDoorPosition == doorSpawnNumber)
            {
                doorLocation = doors[i].gameObject.GetComponent<DoorTriggerInteraction>().spawnLocation;
                _playerSpawnPosition = doorLocation.transform.position;
                //calculate spawn position
                return;
            }
        }
    }
}
