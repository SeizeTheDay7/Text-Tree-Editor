using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TTActor : MonoBehaviour
{
    public string actorName;
    public string teamName;
    public List<TTEvent> eventList;

    void Awake()
    {

    }

    private void SetUpTTEvents()
    {
        // TODO :: eventList 대신 dictionary로 캐싱
    }

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