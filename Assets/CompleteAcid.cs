using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteAcid : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Instance.LeverPrison1 && GameManager.Instance.LeverPrison2)
        {
            this.gameObject.SetActive(false);
        }
    }
}
