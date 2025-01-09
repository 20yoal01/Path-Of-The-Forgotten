using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraBoundsManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        CinemachineConfiner2D confiner2D = gameObject.GetComponent<CinemachineConfiner2D>();
        confiner2D.m_BoundingShape2D = GameObject.FindGameObjectWithTag("Camera Bounds").GetComponent<Collider2D>();
    }
}
