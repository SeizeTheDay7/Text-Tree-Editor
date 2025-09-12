using UnityEngine;
using System.Collections;

public enum GameState
{
    None,
    ReadingText,
    Choice
}

public class GameManager : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] ChoiceButton[] choiceButtons;
    [SerializeField] TextFieldUI textField;

    [Header("Components")]
    [SerializeField] SpriteUtil Intro;
    [SerializeField] TTNarrator narrator;

    [Header("Parameters")]
    [SerializeField] float introDelay = 3f;


    [Header("Cache")]
    GameState gameState;


    void Start()
    {
        StartCoroutine(CoIntroSeq());
    }

    IEnumerator CoIntroSeq()
    {
        float fadeTime = Intro.FadeOut(introDelay);
        yield return new WaitForSeconds(introDelay + fadeTime);
        // TTEditor 첫번째 노드의 텍스트를 가져와서 출력한 뒤, 노드 이벤트 실행
        GoToNextTTNode();
    }

    void Update()
    {
        bool pressEnter = Input.GetKeyDown(KeyCode.Return);

        switch (gameState)
        {
            case GameState.ReadingText:
                if (pressEnter)
                {
                    if (textField.showCoroutine == null) GoToNextTTNode();
                    else textField.SkipText();
                }
                break;
            case GameState.Choice:
                break;
            default:
                if (pressEnter) GoToNextTTNode();
                break;
        }
    }

    private void ChangeState(GameState state)
    {
        gameState = state;
    }

    public void QueueNextNode(float time)
    {
        StartCoroutine(CoQueueNextNode(time));
    }

    private IEnumerator CoQueueNextNode(float time)
    {
        yield return new WaitForSeconds(time);
        GoToNextTTNode();
    }

    private void GoToNextTTNode()
    {
        // Get a smaple from narrator and check the type or null state.
        var nextNode = narrator.GetNextNodeSample();
        if (nextNode == null) return;

        NodeType nextNodeType = nextNode.type;
        if (nextNodeType == NodeType.Text)
        {
            textField.ShowText(narrator.GetActorByName(nextNode.actorName), nextNode.text);
            DoNodeEvents(nextNode);
            narrator.GoToNextNode(nextNode);
            ChangeState(GameState.ReadingText);
        }
        else if (nextNodeType == NodeType.Choice)
        {
            DoNodeEvents(nextNode); // when try to hide text field
            var currentNode = narrator.GetCurrentNode();
            int idx = 0;
            foreach (var edge in currentNode.edgeList)
            {
                var nextChoiceNode = narrator.GetNodeByKey(edge.nextKey);
                choiceButtons[idx].SetNodeAndShow(nextChoiceNode, this);
                idx++;
            }
            ChangeState(GameState.Choice);
        }
    }

    // Called by allocated button event
    public void ChooseNode(TextNodeData node)
    {
        foreach (var btn in choiceButtons) { btn.HideNode(); }
        narrator.GoToNextNode(node); // Go to selected node first
        GoToNextTTNode(); // Go to following node subsequently
    }

    private void DoNodeEvents(TextNodeData node)
    {
        // TODO :: Caching using dictionary
        foreach (TTDEvent ttevent in node.nodeEventList)
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
