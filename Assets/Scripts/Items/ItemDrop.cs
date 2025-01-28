using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    [SerializeField]
    public GameObject itemPrefab;
    [Range(0, 100)] public float dropChance;
}
