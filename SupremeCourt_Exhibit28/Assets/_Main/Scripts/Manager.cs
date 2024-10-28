using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Manager : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Slider slider;
    [SerializeField] private Image filler;

    [SerializeField] private Animator[] videoPlayerAnimators;
    public void OnSliderValueChange()
    {
        scrollBar.value = slider.value;
        filler.fillAmount = Remap(slider.value, 0, 1, 1, 0);
    }

    public void OnScrollBarValueChange()
    {
        slider.value = scrollBar.value;
    }

    public void ButtonSlider(float value)
    {
        slider.value += value;
    }


    private float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

    private void PlayAnimator(bool value)
    {
        foreach (Animator i in videoPlayerAnimators)
        {
            i.SetBool("IsPlaying", value);
        }
    }

    private bool animationState;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            animationState = !animationState;
            PlayAnimator(animationState);
        }
    }




}