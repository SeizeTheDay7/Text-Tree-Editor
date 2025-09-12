using System;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI textUI;
    public TextNodeData node { get; private set; }
    private GameManager gm;
    [SerializeField] float shrinkTime = 0.2f;
    [SerializeField] float shirinkSize = 0.95f;
    [SerializeField] float expandTime = 0.25f;
    [SerializeField] float expandSize = 1.05f;
    [SerializeField] float reblinkTime = 0.1f;

    Color btnColor;
    Color txtColor;

    public void SetNodeAndShow(TextNodeData node, GameManager gm)
    {
        this.gm = gm;
        this.node = node;
        transform.localScale = new Vector3(1f, 1f, 1f);
        btnColor = buttonImage.color;
        buttonImage.color = new Color(btnColor.r, btnColor.g, btnColor.b, 1f);
        txtColor = textUI.color;
        textUI.color = new Color(txtColor.r, txtColor.g, txtColor.b, 1f);
        textUI.text = node.text;
        gameObject.SetActive(true);
    }

    public void Choose()
    {
        StartCoroutine(CoChoose());
    }

    private IEnumerator CoChoose()
    {
        // 줄어들었다가 > 원래보다 커졌다가 > 투명도 0 됐다가 > 아주 약간 불투명해지다가 > 버튼 선택 완료

        float elapsedTime = 0f;
        Vector3 originalScale = transform.localScale;
        Color textColor = textUI.color;
        Color buttonColor = buttonImage.color;

        // 1. 줄어들기
        while (elapsedTime < shrinkTime)
        {
            transform.localScale = Vector3.Lerp(originalScale, originalScale * shirinkSize, elapsedTime / shrinkTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale * shirinkSize;

        // 2. 커지기
        elapsedTime = 0f;
        while (elapsedTime < expandTime)
        {
            transform.localScale = Vector3.Lerp(originalScale * shirinkSize, originalScale * expandSize, elapsedTime / expandTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale * expandSize;

        // 3. 투명도 0으로 갔다가 다시 빠르게 1로 갔다가 다시 0으로
        elapsedTime = 0f;
        while (elapsedTime < reblinkTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / reblinkTime);
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);
            textUI.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < reblinkTime)
        {
            float alpha = Mathf.Lerp(0f, 1, elapsedTime / reblinkTime);
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);
            textUI.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < reblinkTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / reblinkTime);
            buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, alpha);
            textUI.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonImage.color = new Color(buttonColor.r, buttonColor.g, buttonColor.b, 0f);
        textUI.color = new Color(textColor.r, textColor.g, textColor.b, 0f);

        gm.ChooseNode(node);
    }

    public void HideNode()
    {
        gameObject.SetActive(false);
    }
}