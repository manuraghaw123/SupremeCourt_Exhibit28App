using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Video;

[Serializable]
public class InfoText
{
    public List<Infos> infos;
}

[Serializable]
public class Infos
{
    public string name;
    public string info1;
    public string info2;
    public string info3;
    public string info4;
    public string info5;
}

public class Manager : MonoBehaviour
{
    public static Manager instance;


    public enum BuildType
    { 
      PresidentOath,
      CJIOath
    }


    public BuildType buildType;
   
    public InfoText infoText;

    [SerializeField] private Transform canvasTransorm;

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
    [SerializeField] private VideoPlayer vidPlayer;
    [SerializeField] private Image videoButton;
    [SerializeField] private Sprite[] videoButtonSprite;
    [SerializeField] private bool animationState;

    private int videoIndex;
    private int tempIndex = -1;
    private bool videoStatus;
    private  bool sliderReset;
    private string folderName;
    

    private void Awake()
    {
        instance = this;

        ConfigManager.instance.currentVolume += OnVolumeChange;

        if (buildType == BuildType.PresidentOath)
        {
            folderName = "/President";
        }
        else
        {
            folderName = "/CJI";
        }

        vidPlayer.loopPointReached += EndReached;

        string json = File.ReadAllText(Application.streamingAssetsPath + folderName+"/InfoText.json");
        infoText = JsonUtility.FromJson<InfoText>(json);

        for (int i = 0; i < buttonsParentTransform.childCount; i++)
        {
         
            buttonsParentTransform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text =
            infoText.infos[i].name;
        }

        StartCoroutine(LoadVideoButtonTextures());
        StartCoroutine(LoadVideoSelectionSprite());
    }

    private void Update()
    {
        if (sliderReset)
        {
            slider.value = 1;
        }
    }

    #region slider and scroll

    private void ResetSliderOff()
    {
        sliderReset = false;
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

    public void ResetScrollView()
    {
        sliderReset = true;

        foreach (Transform child in buttonsParentTransform)
        {
            child.GetComponent<Image>().sprite = act_deactive_sprite[0];
        }

        Invoke(nameof(ResetSliderOff), 0.2f);
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

    #endregion

    private void OnVolumeChange(float value)
    {
        vidPlayer.SetDirectAudioVolume(0, value);
    }
    private void PlayAnimator(bool value)
    {
        foreach (Animator i in videoPlayerAnimators)
        {
            i.SetBool("IsPlaying", value);
        }
    }
    private IEnumerator LoadVideoButtonTextures()
    {
        yield return null;
        var files = Directory.GetFiles(Application.streamingAssetsPath +folderName + "/Images/ButtonImages/", "*.*", SearchOption.AllDirectories);
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
        var files = Directory.GetFiles(Application.streamingAssetsPath + folderName + "/Images/SelectedImages/", "*.*", SearchOption.AllDirectories);
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

        if (index >= 0 && index < videoSelectionSprites.Count)
        {
            videoSelectImage.texture = videoSelectionSprites[index];
        }

        videoIndex = index;

        Invoke(nameof(SwitchToVideoScreen), 0.5f);

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
    private void SwitchToVideoScreen()
    {
        canvasTransorm.GetChild(2).gameObject.SetActive(false);
        canvasTransorm.GetChild(3).gameObject.SetActive(true);
    }
    private void SwitchToSelectionScreen()
    {
        canvasTransorm.GetChild(3).gameObject.SetActive(false);
        canvasTransorm.GetChild(2).gameObject.SetActive(true);
        videoButton.sprite = videoButtonSprite[1];

    }
    private void PlayVideo(int index)
    {
        if (tempIndex != index)
        {
            vidPlayer.url = Application.streamingAssetsPath + folderName + "/videos/" + index.ToString() + ".mp4";
        }
       
        vidPlayer.Play();
        if (!animationState)
        {
            animationState = true;
            PlayAnimator(animationState);
        }
        else
        {
            videoButton.sprite = videoButtonSprite[1];
        }

        tempIndex = index;

    }
    private void PauseVideo()
    {
        vidPlayer.Pause();
        videoButton.sprite = videoButtonSprite[0];
    }
    private void StopVideo()
    {
        if (vidPlayer.isPlaying)
        {
            vidPlayer.Stop();
        }

        if (animationState)
        {
          animationState = false;
          PlayAnimator(false);
        }

       
        videoButton.sprite = videoButtonSprite[1];
        tempIndex = -1;
        videoStatus = false;

      
    }
    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        StopVideo();
    }
    public void Home()
    {
        foreach (Transform child in canvasTransorm)
        {
            child.gameObject.SetActive(false);
        }

        canvasTransorm.GetChild(0).gameObject.SetActive(true);
    }
    private void OnApplicationQuit()
    {
        vidPlayer.loopPointReached -= EndReached;
        ConfigManager.instance.currentVolume += OnVolumeChange;
    }
    public void VideoStatus()
    {
        videoStatus = !videoStatus;

        if (videoStatus)
        {
            PlayVideo(videoIndex + 1);
        }
        else
        {
            PauseVideo();
        }
    }
    public void BackFromVideo()
    {
        if (animationState)
        {
            StopVideo();
            Invoke(nameof(SwitchToSelectionScreen), 1.3f);
        }
        else
        {
            SwitchToSelectionScreen();
        }
    }
    public void BackFromHome()
    {
        if (animationState)
        {
            StopVideo();
            Invoke(nameof(Home), 1.3f);
        }
        else
        {
            Home();
        }
    }
}
