using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PageTransition : MonoBehaviour
{
    public CanvasGroup currentCanvasGroup;
    public CanvasGroup targetCanvasGroup;
    public float fadeDuration = 1f;
    public UnityEvent afterTransitionComplete;
   

    public void Transition()
    {
        StartCoroutine(FadeOutIn(currentCanvasGroup, targetCanvasGroup));
    }

    private IEnumerator FadeOutIn(CanvasGroup currentPage, CanvasGroup targetPage)
    {
        float elapsedTime = 0f;
        float currentPageStartAlpha = currentPage.alpha;
        float targetPageStartAlpha = targetPage.alpha;
        targetPage.alpha = 0f;
        targetPage.gameObject.SetActive(true);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            currentPage.alpha = Mathf.Lerp(currentPageStartAlpha, 0f, t);
            targetPage.alpha = Mathf.Lerp(targetPageStartAlpha, 1f, t);

            yield return null;
        }
        currentPage.alpha = 0f;
        targetPage.alpha = 1f;

        afterTransitionComplete.Invoke();
    }

    


}


