using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    public GameObject player { get; set; }
    Collider2D col;
    Animator animator;
    public bool savedState;
    Damageable damageable;
    public string id;

    private void Awake()
    {
        damageable = GameObject.FindWithTag("Player").GetComponent<Damageable>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (savedState)
        {
            animator.SetBool(AnimationString.ActiveCheckpoint, true);
            GameManager.Instance.currentCheckPointId = this.id;
        }
    }

    private void OnEnable()
    {
        damageable.deathEvent.AddListener(RespawnPlayer);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        damageable.deathEvent.RemoveListener(RespawnPlayer);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (savedState)
            return;

        if (GameManager.Instance.currentCheckPointId != null)
        {
            Respawn currentCheckpoint = Respawn.FindRespawnByID(GameManager.Instance.currentCheckPointId);
            currentCheckpoint.animator.SetBool(AnimationString.ActiveCheckpoint, false); // Deactivate last checkpoint
            currentCheckpoint.savedState = false;
        }

        GameManager.Instance.currentCheckPointId = this.id;
        animator.SetBool(AnimationString.ActiveCheckpoint, true);
        InputManager.Instance.SaveAsync();
        savedState = true;
    }

    private void RespawnPlayer()
    {
        if (savedState)
        {
            StartCoroutine(FadeOutThenRespawn());
            SceneFadeManager.instance.StartFadeIn();
            damageable.Revive();
            InputManager.Instance.ActivatePlayerControls();
        }
    }

    private IEnumerator FadeOutThenRespawn()
    {
        InputManager.Instance.DeactivatePlayerControls();
        SceneFadeManager.instance.StartFadeOut();
        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }
        InputManager.Instance.LoadAsync();
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (id == GameManager.Instance.currentCheckPointId)
        {
            savedState = true;
            animator.SetBool(AnimationString.ActiveCheckpoint, true);
        }
    }

    public void Save(ref RespawnData data)
    {
        data.id = id;
    }

    public void Load(RespawnData data)
    {
        id = data.id;
    }

    public static Respawn FindRespawnByID(string respawnID)
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Respawn");
        Respawn respawn = null;

        foreach (var checkpoint in checkpoints)
        {
            respawn = checkpoint.GetComponent<Respawn>();
            Animator respawnAnimator = checkpoint.GetComponent<Animator>();
            if (respawn.id == respawnID)
            {
                break;
            }
        }

        return respawn;
    }
}

[System.Serializable]
public struct RespawnData
{
    public string id;
}