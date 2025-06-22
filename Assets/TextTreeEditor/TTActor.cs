using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TTActor : MonoBehaviour
{
    public string actorName;
    public List<TTEvent> eventList;

    public void InvokeTTEvent(string eventName)
    {
        foreach (TTEvent ttevent in eventList)
        {
            if (ttevent.eventName == eventName)
            {
                ttevent.eventdata.Invoke();
                return;
            }
        }
    }
}