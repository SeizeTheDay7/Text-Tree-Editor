using System.Collections.Generic;
using UnityEngine;

public class TTNarrator : MonoBehaviour
{
    public TextTreeSO textTreeSO;
    public List<TTActor> actorList;
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