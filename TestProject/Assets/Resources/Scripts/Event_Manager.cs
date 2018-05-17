using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages Event generation and subscription.
/// </summary>
public static class Event_Manager {

    // Stores the delegates that get called when an event is fired
    private static Dictionary<Event_Trigger, Action<Character_Action, string, GameObject>> events = new Dictionary<Event_Trigger, Action<Character_Action, string, GameObject>>();

    /// <summary>
    /// Adds a Method to a specific Event_Trigger.
    /// </summary>
    /// <param name="evnt">The Event_Trigger to subscribe to.</param>
    /// <param name="action">The Action to add.</param>
    public static void AddHandler(Event_Trigger evnt, Action<Character_Action, string, GameObject> action)
    {
        if (!events.ContainsKey(evnt))
        {
            events[evnt] = action;
        }
        else
        {
            events[evnt] += action;
        }
    }

    /// <summary>
    /// Removes a Method from a specific Event_Trigger.
    /// </summary>
    /// <param name="evnt">The Event_Trigger from which to remove the Action.</param>
    /// <param name="action">The Action to remove.</param>
    public static void RemoveHandler(Event_Trigger evnt, Action<Character_Action, string, GameObject> action)
    {
        if (events[evnt] != null)
        {
            events[evnt] -= action;
        }
        else {
            events.Remove(evnt);
        }
    }

    /// <summary>
    /// Method called to fire the Event. Will trigger all methods subscribed to the specified Event_Trigger.
    /// </summary>
    /// <param name="evnt">The Event_Trigger whose methods you want to trigger</param>
    /// <param name="act">The Character_Action being performed to trigger the event. Used to parse the string value.</param>
    /// <param name="value">A string containing the value of the given Action. May contain Acceptable_Shortcuts</param>
    /// <param name="target">The target of the Character_Action</param>
    public static void Broadcast(Event_Trigger evnt, Character_Action act, string value, GameObject target)
    {
        if (events[evnt] != null)
        {
            events[evnt](act,value,target);
        }
    }
}
