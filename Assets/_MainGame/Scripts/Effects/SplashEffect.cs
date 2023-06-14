using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffect : MonoBehaviour , IColorable
{
    public Color baseColor;
    [SerializeField] private float TimeFaded;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private Sprite[] splashSprArray;
    [SerializeField] private ParticleSystem[] particleArray;

    public void OnEnable()
    {
        RandomSplash();
        StartCoroutine(C_Faded());
    }

    private IEnumerator C_Faded()
    {
        float timeFadedFixed = TimeFaded * 0.5f;
        yield return new WaitForSecondsRealtime(timeFadedFixed);

        float t = 0.0f;
        Color a = baseColor;
        Color b = a;
        b.a = 0.0f;
        while (t < 1.0f)
        {
            t += (Time.unscaledDeltaTime / timeFadedFixed);
            Color c = Color.Lerp(a, b, t);
            spr.color = c;
            yield return null;
        }
    }

    private void RandomSplash()
    {
        spr.sprite = splashSprArray[Random.Range(0, splashSprArray.Length)];
        spr.color = baseColor;
    }

    public void SetColor(Color color)
    {
        baseColor = color;
        spr.color = baseColor;

        for (int i = 0; i < particleArray.Length; i++)
        {
            var a = particleArray[i].main;
            a.startColor = baseColor;
        }
    }
}
