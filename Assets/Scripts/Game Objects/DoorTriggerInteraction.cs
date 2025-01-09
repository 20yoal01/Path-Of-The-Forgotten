using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerInteraction : MonoBehaviour
{
    [Header("Spawn To:")]
    [SerializeField] private SceneField _sceneToLoad;

    [SerializeField] private BoxCollider2D areaToTeleport;
    [SerializeField] private DoorToSpawnAt DoorToSpawnTo;

    [Space(10f)]
    [Header("Current Door")]
    public DoorToSpawnAt CurrentDoorPosition;
    [SerializeField]
    public Transform spawnLocation;
    
    public enum DoorToSpawnAt
    {
        None,
        One,
        Two,
        Three,
        Four,
        Five,
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (areaToTeleport == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            SceneSwapManager.SwapSceneFromDoorUse(_sceneToLoad, DoorToSpawnTo);
        }
    }
}
