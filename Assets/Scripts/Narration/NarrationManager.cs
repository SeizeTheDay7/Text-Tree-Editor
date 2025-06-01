using TMPro;
using UnityEngine;
using Febucci.UI;

public class NarrationManager : MonoBehaviour
{
    // [SerializeField] private TextMeshProUGUI textHolder;
    [SerializeField] private NarrationSO startScript;
    [SerializeField] private TypewriterByCharacter typewriter;
    private NarrationSO currentScript;
    private int idx = 0;

    void Start()
    {
        currentScript = startScript;
        typewriter.ShowText(currentScript.narrationText[idx]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (typewriter.isShowingText)
            {
                typewriter.SkipTypewriter();
            }
            else
            {
                idx++;
                if (idx < currentScript.narrationText.Length)
                {
                    typewriter.ShowText(currentScript.narrationText[idx]);
                }
                else
                {
                    idx = 0;
                    currentScript = NextBranch(currentScript);
                    typewriter.ShowText(currentScript.narrationText[idx]);
                }
            }
        }
    }

    private NarrationSO NextBranch(NarrationSO currentScript)
    {
        if (currentScript.nextScript.Length == 1)
        {
            return currentScript.nextScript[0].branchNode;
        }
        else // TODO :: add condition check 
        {
            return currentScript.nextScript[0].branchNode;
        }
    }
}
