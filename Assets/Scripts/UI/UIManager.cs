using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject guidingTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindAnyObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterHealed;
        CharacterEvents.characterAbilityUnlock += CharacterAbilityUnlock;
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -=  CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterHealed;
        CharacterEvents.characterAbilityUnlock -= CharacterAbilityUnlock;
    }


    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        // Create text at character hit
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterAbilityUnlock(GameObject character, Ability ability)
    {
        // Set the spawn position to the center of the screen
        Vector3 spawnPosition = new Vector3(Screen.width / 2 + 100, Screen.height / 2, 0);

        // Instantiate the text object at the spawn position within the game canvas
        TMP_Text tmpText = Instantiate(guidingTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        // Set the text content
        tmpText.text = "New " + ability.ToString().ToLower() + " ability unlocked!" + $" {AbilityManager.GetKeybind(ability)}";
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        // Create text at character hit
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
                Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            #endif

            #if (UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;
            #elif (UNITY_STANDALONE)
                                Application.Quit();
            #elif (UNITY_WEBGL)
                                SceneManager.LoadScene("QuitScene);
            #endif
        }
    }
}
