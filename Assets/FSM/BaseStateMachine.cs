using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateMachine : MonoBehaviour
{
    [SerializeField] private BaseState _initialState;
    public BaseState CurrentState { get; set; }

    private void Awake()
    {
        CurrentState = _initialState;
    }

    void Start()
    {
        CurrentState.Enter(this);
    }

    void Update()
    {
        CurrentState.Execute(this);
    }
}
