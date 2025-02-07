using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ProjectileSpawn", story: "Boss fires [Fireball] at [Player] inside [BossRoom]", category: "Action", id: "1473a2ce8336d508202fa21789870307")]
public partial class ProjectileSpawnAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Fireball;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<GameObject> BossRoom;
    private BoxCollider2D _bossRoomCollider;

    protected override Status OnStart()
    {
        if (!BossRoom.Value.TryGetComponent(out _bossRoomCollider))
        {
            Debug.LogError("Could not find a Collider2D in your BossRoom!");
            return Status.Failure;
        }

        if (Player == null)
        {
            Debug.LogError("Please Specify your Player");
            return Status.Failure;
        }

        if (_bossRoomCollider != null)
        {
            // Get the size and center of the collider
            Vector2 roomSize = _bossRoomCollider.size;
            Vector2 roomCenter = _bossRoomCollider.bounds.center;

            float start_x = roomCenter.x - roomSize.x / 2;
            float end_x = roomCenter.x + roomSize.x / 2;

            float start_y = roomCenter.y - roomSize.y / 2;
            float end_y = roomCenter.y + roomSize.y / 2;

            // Generate a random position within the BoxCollider2D's bounds
            
            float randomX = UnityEngine.Random.Range(start_x, end_x);
            float randomY = UnityEngine.Random.Range(start_y, end_y);

            Vector2 spawnPosition = new Vector2(randomX, randomY);

            Vector2 playerPosition = Player.Value.transform.position;
            float distanceToPlayer = Vector2.Distance(spawnPosition, playerPosition);
            if (distanceToPlayer < 10f)
            {
                Vector2 directionToPlayer = (spawnPosition - playerPosition).normalized;
                spawnPosition = playerPosition + directionToPlayer * 10f;
            }

            // Instantiate the Fireball at the random position
            if (Fireball.Value != null)
            {
                GameObject fireballInstance = UnityEngine.Object.Instantiate(Fireball.Value, spawnPosition, Quaternion.identity);
            }
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

