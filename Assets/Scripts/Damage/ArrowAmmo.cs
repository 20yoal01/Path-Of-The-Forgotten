using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowAmmo : MonoBehaviour
{
    public TMP_Text ammoText;

    PlayerController playerController;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has tag 'Player'");
        }

        playerController = player.GetComponent<PlayerController>();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController.ammoRemaningEvent.AddListener(OnPlayerAmmoChanged);

        ammoText.text = "x" + playerController.ArrowsRemaining;
        if (!playerController.canShootBow)
            gameObject.SetActive(false);
    }

    public void OnBowAbilityUnlock()
    {
        if (playerController.canShootBow)
            gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        playerController.ammoRemaningEvent.AddListener(OnPlayerAmmoChanged);
    }

    private void OnDisable()
    {
        playerController.ammoRemaningEvent.RemoveListener(OnPlayerAmmoChanged);
    }

    private void OnPlayerAmmoChanged(int ammo)
    {
        ammoText.text = "x" + playerController.ArrowsRemaining;
    }
}
