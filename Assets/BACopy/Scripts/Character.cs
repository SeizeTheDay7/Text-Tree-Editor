using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    [Header("Components")]
    SpriteRenderer sr;
    float targetPosX;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void LoseFocus(Color color)
    {
        sr.color = color;
    }

    public void GetFocus()
    {
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    public void SetPosX(float x)
    {
        Vector3 pos = transform.localPosition;
        pos.x = x;
        transform.localPosition = pos;
    }

    public void SetTargetPosX(float x)
    {
        targetPosX = x;
    }

    public void MoveToTargetPos(float time)
    {
        StartCoroutine(CoMoveToTargetPos(time));
    }

    private IEnumerator CoMoveToTargetPos(float time)
    {
        float elapsedTime = 0f;
        Vector3 originalPos = transform.localPosition;
        Vector3 targetPos = new Vector3(targetPosX, originalPos.y, originalPos.z);

        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(originalPos, targetPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;
    }

    public void DipOnce(float time)
    {
        StartCoroutine(CoDipOnce(time));
    }

    private IEnumerator CoDipOnce(float time)
    {
        yield return new WaitForSeconds(1f);

        // 한 번 아래로 -2만큼 찍고 오기
        float elapsedTime = 0f;
        Vector3 originalPos = transform.localPosition;
        Vector3 targetPos = new Vector3(originalPos.x, originalPos.y - 2f, originalPos.z);
        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(originalPos, targetPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPos;
        elapsedTime = 0f;
        while (elapsedTime < time)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalPos, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}