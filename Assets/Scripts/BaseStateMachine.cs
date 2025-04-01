using System;
using UnityEngine;

public abstract class BaseStateMachine<T> : MonoBehaviour where T : BaseState
{
    public abstract void ChangeState(T newState);

    public abstract void Start();

    public abstract void Update();
}
