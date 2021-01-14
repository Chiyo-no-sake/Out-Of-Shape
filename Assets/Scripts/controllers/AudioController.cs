using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource _source;
    // Start is called before the first frame update
    void Start()
    {
        _source = AudioManager.GetInstance().GetSource();
        
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        _source.volume = Mathf.Lerp(_source.volume, 0.0f, 0.2f);
        yield return new WaitForEndOfFrame();
    }

    private IEnumerator FadeInRoutine()
    {
        _source.volume = Mathf.Lerp(_source.volume, 1.0f, 0.2f);
        yield return new WaitForEndOfFrame();
    }
}
