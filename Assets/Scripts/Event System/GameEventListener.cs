using System;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<Component, object> { }

[System.Serializable]
public struct GameEventPair
{
    public GameEvent gameEvent;
    public CustomGameEvent response;
}

public class GameEventListener : MonoBehaviour
{
    public GameEventPair[] eventPairs;
    
    private void OnEnable()
    {
        foreach (var pair in eventPairs)
        {
            pair.gameEvent.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        foreach (var pair in eventPairs)
        {
            pair.gameEvent.UnregisterListener(this);
        }
    }

    public void OnEventRaised(GameEvent gameEvent, Component sender, object data)
    {
        foreach (var pair in eventPairs)
        {
            if (pair.gameEvent == gameEvent)
            {
                pair.response.Invoke(sender, data);
                break;
            }
        }
    }
}