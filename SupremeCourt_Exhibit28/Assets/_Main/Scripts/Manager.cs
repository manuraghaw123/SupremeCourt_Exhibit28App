using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

[Serializable]
public class InfoText
{
    public List<Infos> infos;
}

[Serializable]
public class Infos
{
    public string info1;
    public string info2;
    public string info3;
    public string info4;
    public string info5;
}

public class Manager : MonoBehaviour
{
    public static Manager instance;

    public InfoText infoText;

    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Slider slider;
    [SerializeField] private Image filler;
    [SerializeField] private Transform buttonsParentTransform;
    [SerializeField] private Animator[] videoPlayerAnimators;
    [SerializeField] private List<Texture2D> videoButtonTextures;
    [SerializeField] private List<Texture2D> videoSelectionSprites;
    [SerializeField] private RawImage videoSelectImage;
    [SerializeField] private Sprite[] act_deactive_sprite;

    [SerializeField] private Transform bigTextHolder, smallTextHolder;
    

    private void Awake()
    {
        instance = this;

        string json = File.ReadAllText(Application.streamingAssetsPath + "/InfoText.json");
        infoText = JsonUtility.FromJson<InfoText>(json);

        StartCoroutine(LoadVideoButtonTextures());
        StartCoroutine(LoadVideoSelectionSprite());
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
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Images/ButtonImages/", "*.*", SearchOption.AllDirectories);
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

    private IEnumerator LoadVideoSelectionSprite()
    {
        yield return null;
        var files = Directory.GetFiles(Application.streamingAssetsPath + "/Images/SelectedImages/", "*.*", SearchOption.AllDirectories);
        foreach (string filename in files)
        {
            if (Regex.IsMatch(filename, @".jpg|.png|.jpeg$"))
                if (!filename.Contains(".meta"))
                {
                    var rawData = File.ReadAllBytes(filename);
                    Texture2D tex = new(2, 2);
                    tex.LoadImage(rawData);
                  
                    videoSelectionSprites.Add(tex);
                }
        }
    }

    public void SelectVideoImage(int index)
    {
        foreach (Transform child in buttonsParentTransform)
        {
            child.GetComponent<Image>().sprite = act_deactive_sprite[0];
        }
        buttonsParentTransform.GetChild(index).GetComponent<Image>().sprite = act_deactive_sprite[1];

        SelectText(index);

        videoSelectImage.texture = videoSelectionSprites[index];
    }

    private void SelectText(int index)
    {
        bigTextHolder.GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info1;
        bigTextHolder.GetChild(1).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info2;
        bigTextHolder.GetChild(2).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info3;
        bigTextHolder.GetChild(3).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info4;
        bigTextHolder.GetChild(4).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info5;

        smallTextHolder.GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info1;
        smallTextHolder.GetChild(1).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info2;
        smallTextHolder.GetChild(2).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info3;
        smallTextHolder.GetChild(3).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info4;
        smallTextHolder.GetChild(4).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].info5;
    }
   





}
