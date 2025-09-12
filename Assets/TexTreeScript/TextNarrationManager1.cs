using UnityEngine;
using Febucci.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TextNarrationManager1 : MonoBehaviour
{
    [SerializeField] TTNarrator narrator;
    [SerializeField] private GameObject narratorField;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TypewriterByCharacter typewriter;
    [SerializeField] private GameObject choiceField;
    [SerializeField] private TextMeshProUGUI buttontext1;
    [SerializeField] private TextMeshProUGUI buttontext2;

    Dictionary<string, TextNodeData> ttdict;
    TextNodeData currentNode;
    Mode mode;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mode = Mode.narration;
        SetupTTSO();
        currentNode = ttdict[narrator.textTreeSO.initNodeKey];
        title.text = currentNode.actorName;
        typewriter.ShowText(currentNode.text);
    }

    private void SetupTTSO()
    {
        ttdict = new Dictionary<string, TextNodeData>();
        foreach (var nodeData in narrator.textTreeSO.textNodeList)
        {
            ttdict[nodeData.key] = nodeData;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.narration && Input.GetKeyDown(KeyCode.Return))
        {
            if (typewriter.isShowingText)
            {
                typewriter.SkipTypewriter();
            }
            else
            {
                InvokeCurrentNodeEvent();
                if (mode != Mode.narration) return;

                currentNode = ttdict[currentNode.edgeList[0].nextKey];
                title.text = currentNode.actorName;
                typewriter.ShowText(ttdict[currentNode.key].text);
            }
        }
    }

    public void Choice(int idx)
    {
        currentNode = ttdict[currentNode.edgeList[idx].nextKey];
        currentNode = ttdict[currentNode.edgeList[0].nextKey];
        title.text = currentNode.actorName;
        ChangeToNarration();
        typewriter.ShowText(ttdict[currentNode.key].text);
    }

    public void ChangeToNarration()
    {
        mode = Mode.narration;

        choiceField.SetActive(false);
        narratorField.SetActive(true);
    }
    public void ChangeToChoice()
    {
        mode = Mode.choice;

        narratorField.SetActive(false);
        choiceField.SetActive(true);

        List<TextEdge> edges = currentNode.edgeList;

        buttontext1.text = ttdict[edges[0].nextKey].text;
        buttontext2.text = ttdict[edges[1].nextKey].text;
    }

    private void InvokeCurrentNodeEvent()
    {
        foreach (TTDEvent ttevent in currentNode.nodeEventList)
        {
            foreach (TTActor actor in narrator.actorList)
            {
                if (actor.actorName == ttevent.actorName)
                {
                    actor.InvokeTTEvent(ttevent.eventName);
                    break;
                }
            }
        }
    }
}
