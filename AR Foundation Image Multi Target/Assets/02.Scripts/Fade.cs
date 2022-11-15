using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [Header("FadeOut")]
    // 동적 프리팹 이미지 컴포넌트
    Image fadeImg;
    // Fade 효과 재생시간
    public float fadeTime = 2f;
    float currectTime = 0f;
    float fadeStartF = 1f;
    float fadeEndF = 0f;

    private void Awake()
    {
        // Image BackGround 오브젝트에 컴포넌트로 fadeImg 지정
        fadeImg = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        Color fadeColor = fadeImg.color;
        currectTime = 0f;
        fadeColor.a = Mathf.Lerp(fadeStartF, fadeEndF, currectTime);
        if (fadeColor.a > 0f)
        {
            currectTime += Time.deltaTime / fadeTime;
            fadeColor.a = Mathf.Lerp(fadeStartF, fadeEndF, currectTime);
            fadeImg.color = fadeColor;
        }
    }

    private void OnDisable()
    {
        currectTime = 0f;
    }
}
