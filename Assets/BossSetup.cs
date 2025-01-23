using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSetup : MonoBehaviour
{
    public GameObject bossUI;

    private void Awake()
    {
        if (bossUI != null)
        {
            bossUI.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && bossUI != null)
        {
            bossUI.gameObject.SetActive(true);
        }
    }
}
