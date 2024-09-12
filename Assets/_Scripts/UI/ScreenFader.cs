using System.Collections;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ScreenFade : Singleton<ScreenFade>
    {
        private Coroutine fadeCoroutine;
        private bool isFading;
        private Image fadeImage;

        protected override void Awake()
        {
            base.Awake();
            fadeImage = GetComponent<Image>();
        }

        public void FadeOut(float duration, Color fadeColor)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeImage.raycastTarget = true;
            fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration,default));
        }

        public void FadeIn(float duration)
        {
            fadeImage.raycastTarget = true;
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeInCoroutine(duration));
        }

        private IEnumerator FadeOutCoroutine(float duration, Color fadeColor)
        {
            isFading = true;
            float alpha = 0;
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            float timer = 0;
            while (timer < duration)
            {
                alpha = Mathf.Lerp(0, 1, timer / duration);
                timer += Time.deltaTime;
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
                yield return null;
            }

            alpha = 1;
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            isFading = false;
        }

        private IEnumerator FadeInCoroutine(float duration)
        {
            isFading = true;
            float alpha = 1;
            float timer = 0;
            Color color = fadeImage.color;
            while (timer < duration)
            {
                alpha = Mathf.Lerp(1, 0, timer / duration);
                timer += Time.deltaTime;
                fadeImage.color = new Color(color.r, color.g, color.b, alpha);
                if(timer/duration > 0.25f)
                {
                    fadeImage.raycastTarget = false;
                }
                yield return null;
            }
            alpha = 0;
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);

            isFading = false;
        }
    }
}
