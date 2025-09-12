using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgm;
    [SerializeField] float bgmFadeInTime = 2f;
    float originVolume;

    void Awake()
    {
        originVolume = bgm.volume;
        BGMStart();
    }

    public void BGMStart()
    {
        StartCoroutine(CoBGMFadeIn());
    }

    private IEnumerator CoBGMFadeIn()
    {
        float elapsedTime = 0f;
        bgm.volume = 0f;
        bgm.Play();

        while (elapsedTime < bgmFadeInTime)
        {
            bgm.volume = Mathf.Lerp(0f, originVolume, elapsedTime / bgmFadeInTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bgm.volume = originVolume;
    }

    public void BGMStop()
    {
        bgm.Stop();
    }

    public void ChangeToBGM(AudioClip clip)
    {
        bgm.clip = clip;
    }
}