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
    public Vector3 savedPlayerPosition;
    public string sceneDataIndex;

    private void Awake()
    {
        GameObject game = GameManager.Instance.gameObject;
        GameObject player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        if (player != null)
        {
            damageable = GameObject.FindWithTag("Player").GetComponent<Damageable>();
        }
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
        if (damageable != null)
        {
            damageable.deathEvent.AddListener(RespawnPlayer);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        if (damageable != null)
        {
            damageable.deathEvent.RemoveListener(RespawnPlayer);
        }       
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (savedState)
        {
            animator.SetBool(AnimationString.ActiveCheckpoint, false);
        }

        if (GameManager.Instance.currentCheckPointId != null && GameManager.Instance.currentCheckPointId != "")
        {
            Respawn currentCheckpoint = Respawn.FindRespawnByID(GameManager.Instance.currentCheckPointId);
            if (currentCheckpoint != null)
            {
                currentCheckpoint.animator.SetBool(AnimationString.ActiveCheckpoint, false); // Deactivate last checkpoint
                currentCheckpoint.savedState = false;
            }
        }

        GameManager.Instance.currentCheckPointId = this.id;
        StartCoroutine(DeactivateAndReactivateCheckpoint());
        savedState = true;

        GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
        if (currentPlayer != null)
        {
            savedPlayerPosition = currentPlayer.transform.position;
            sceneDataIndex = GameManager.Instance.SceneData.Data.UniqueName;
            GameManager.Instance.currentCheckPointSceneIndex = sceneDataIndex;
        }
        
        InputManager.Instance.SaveAsync();
    }

    private IEnumerator DeactivateAndReactivateCheckpoint()
    {
        // Wait until the animation finishes
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Reactivate the checkpoint
        animator.SetBool(AnimationString.ActiveCheckpoint, true);
    }

    private void RespawnPlayer()
    {
        if (damageable != null && damageable.Health <= 0)
        {
            StartCoroutine(FadeOutThenRespawn());
            SceneFadeManager.instance.StartFadeIn();
            damageable.Revive();
            InputManager.Instance.ActivatePlayerControls();
            GameManager.Instance.isRespawning = true;
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

        if (GameManager.Instance.isRespawning && id == GameManager.Instance.currentCheckPointId)
        {
            GameManager.Instance.isRespawning = false;
            GameObject currentPlayer = GameObject.FindGameObjectWithTag("Player");
            if (currentPlayer != null && savedPlayerPosition != null)
            {
                currentPlayer.transform.position = savedPlayerPosition;
            }
        }
    }

    public void Save(ref RespawnData data)
    {
        data.id = id;
        data.savedPlayerPosition = savedPlayerPosition;
        data.sceneDataIndex = sceneDataIndex;
    }

    public void Load(RespawnData data)
    {
        id = data.id;
        savedPlayerPosition = data.savedPlayerPosition;
        sceneDataIndex = data.sceneDataIndex;
    }

    public static Respawn FindRespawnByID(string respawnID)
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Respawn");
        Respawn respawn = null;
        bool foundRespawn = false;

        foreach (var checkpoint in checkpoints)
        {
            respawn = checkpoint.GetComponent<Respawn>();
            Animator respawnAnimator = checkpoint.GetComponent<Animator>();
            if (respawn.id == respawnID)
            {
                foundRespawn = true;
                break;
            }
        }

        return foundRespawn ? respawn : null;
    }
}

[System.Serializable]
public struct RespawnData
{
    public string id;
    public Vector3 savedPlayerPosition;
    public string sceneDataIndex;
}