using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFieldUI : MonoBehaviour
{
    [SerializeField] GameObject textFieldUI;
    [SerializeField] float textShowSpeed = 2f;
    [SerializeField] RectTransform nameRect;
    [SerializeField] TextMeshProUGUI tmpName;
    [SerializeField] TextMeshProUGUI tmpTeam;
    [SerializeField] TextMeshProUGUI tmpText;
    public Coroutine showCoroutine { get; private set; }

    public void ShowText(TTActor actor, string text)
    {
        tmpName.text = actor.actorName;
        tmpText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(nameRect);
        gameObject.SetActive(true);
        showCoroutine = StartCoroutine(CoShowText());
    }

    private IEnumerator CoShowText()
    {
        // 개수 하나씩 늘어남
        tmpText.maxVisibleCharacters = 0;
        tmpText.ForceMeshUpdate();
        int totalCharCount = tmpText.textInfo.characterCount;
        int visibleCount = 0;
        float elapsedTime = 0f;
        while (visibleCount < totalCharCount)
        {
            visibleCount = Mathf.Min(totalCharCount, Mathf.FloorToInt(elapsedTime * textShowSpeed));
            // 만약 이번 문자가 공백이면, 그냥 넘어감
            if (visibleCount < totalCharCount && char.IsWhiteSpace(tmpText.text[visibleCount]))
                visibleCount++;
            tmpText.maxVisibleCharacters = visibleCount;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        showCoroutine = null;
    }

    public void SkipText()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        tmpText.maxVisibleCharacters = tmpText.textInfo.characterCount;
    }

    public void Hide()
    {
        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
            showCoroutine = null;
        }
        gameObject.SetActive(false);
    }
}