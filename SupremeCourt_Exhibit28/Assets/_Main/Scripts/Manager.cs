using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;



public class Manager : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Slider slider;
    [SerializeField] private Image filler;
    [SerializeField] private Transform buttonsParentTransform;
    [SerializeField] private Animator[] videoPlayerAnimators;
    [SerializeField] private List<Texture2D> videoButtonTextures;
    

    private void Awake()
    {
        StartCoroutine(LoadVideoButtonTextures());
    }

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

    private IEnumerator LoadVideoButtonTextures()
    {
        yield return null;
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Images/SelectionImage/", "*.*", SearchOption.AllDirectories);
        foreach (string filename in files)
        {
            if (Regex.IsMatch(filename, @".jpg|.png|.jpeg$"))
                if (!filename.Contains(".meta"))
                {
                    var rawData = File.ReadAllBytes(filename);
                    Texture2D tex = new (2, 2);
                    tex.LoadImage(rawData);
                    videoButtonTextures.Add(tex);
                }
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < videoButtonTextures.Count; i++)
        {
            buttonsParentTransform.GetChild(i).GetChild(0).GetComponent<RawImage>().texture = videoButtonTextures[i];
        }
    }





}
