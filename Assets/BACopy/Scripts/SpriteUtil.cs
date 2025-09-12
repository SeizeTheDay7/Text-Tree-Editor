using UnityEngine;
using System.Collections;
using EditorAttributes;

public class SpriteUtil : MonoBehaviour
{
    [Header("Components")]
    SpriteRenderer sr;

    [Header("Parameters")]
    [SerializeField] float fadeOutDelay = 0f;
    [SerializeField] float fadeOutTime = 1f;


    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public float FadeOut(float fadeOutDelay)
    {
        this.fadeOutDelay = fadeOutDelay;
        StartCoroutine(CoFadeOut());
        return fadeOutTime;
    }

    private IEnumerator CoFadeOut()
    {
        if (fadeOutDelay > 0f)
            yield return new WaitForSeconds(fadeOutDelay);

        float elapsedTime = 0f;
        Color originalColor = sr.color;
        float originalAlpha = originalColor.a;

        while (elapsedTime < fadeOutTime)
        {
            float alpha = Mathf.Lerp(originalAlpha, 0f, elapsedTime / fadeOutTime);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        gameObject.SetActive(false);
    }
}