using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public List<ItemDrop> lootTable = new List<ItemDrop>();

    public void DropItems()
    {
        foreach (ItemDrop item in lootTable)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            bool canShootBow = false;
            if (player != null)
            {
                canShootBow = player.GetComponent<PlayerController>().canShootBow;
            }

            if (item.itemPrefab.name == "Arrow Pickup" && !canShootBow)
            {
                return;
            }
            if (Random.Range(0, 100f) <= item.dropChance)
            {
                Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}
