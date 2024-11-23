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
    public string date;
    public List<Language> language; 
    public string yearTime;
    public bool isVideo;
    public string videoname;
}

[Serializable]
public class Language
{
    public string name;
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

    [SerializeField] private AppLanguage currentLanguage;
    [SerializeField] private TMP_Dropdown[] lanugaeDropdown;

    [SerializeField] private Transform canvasTransorm;

    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private Slider slider;
    [SerializeField] private Image filler;
    [SerializeField] private Transform buttonsParentTransform;
    [SerializeField] private Transform horizontalButtonsParentTransfom;
    [SerializeField] private Color nonSelectedColor;
    [SerializeField] private Animator[] videoPlayerAnimators;
    [SerializeField] private List<Texture2D> videoSelectionSprites;
    [SerializeField] private List<Texture2D> videoRoundTexture;
    [SerializeField] private RawImage videoSelectImage;
    [SerializeField] private Sprite[] act_deactive_sprite;

    [SerializeField] private Transform bigTextHolder, smallTextHolder;
    [SerializeField] private VideoPlayer vidPlayer;
    [SerializeField] private Image videoButton;
    [SerializeField] private Sprite[] videoButtonSprite;
    [SerializeField] private bool animationState;

    [SerializeField] private GameObject[] videoPlayAndPauseButtons;
    [SerializeField] private VideoPlayer holdingVideoPlayer;

    [SerializeField] private PageTransition _PageTransition3;
    [SerializeField] private PageTransition _PageTransition4_BackFromVideo;
    [SerializeField] private PageTransition _PageTransition4_ToHome;

    [SerializeField] private CanvasGroup[] VideoPlayerScreenCanvas;
    [SerializeField] private CanvasGroup seekBarCanavasGroup;
    [SerializeField] private Slider seekBar;
    [SerializeField] private GameObject checker;
    [SerializeField] private Scrollbar horizontalSelectionScrollbar;

    [SerializeField] private GameObject[] EnglishObjects;
    [SerializeField] private GameObject[] HindiObjects;

    [SerializeField] private Image[] screenImages;
    [SerializeField] private Sprite[] englishSprites;
    [SerializeField] private Sprite[] hindiSprites;

    

    private int videoIndex;
    private string tempName = "tempName";
    private bool videoStatus;
    private  bool sliderReset;
    private string folderName;
    private bool isTransition;
    

    private void Awake()
    {
        instance = this;

        foreach (var dropdown in lanugaeDropdown)
        {
            dropdown.onValueChanged.AddListener((index) => OnLanguageChanged(index));
        }


        if (buildType == BuildType.PresidentOath)
        {
            folderName = "/President";
        }
        else
        {
            folderName = "/CJI";
        }

        vidPlayer.loopPointReached += EndReached;

        string json = File.ReadAllText(Application.streamingAssetsPath + folderName+ "/InfoTextNew.json");
        infoText = JsonUtility.FromJson<InfoText>(json);

       

        PlayHoldingScreenVideo();

        StartCoroutine(LoadVideoSelectionSprite());
        StartCoroutine(LoadRoundImages());

        horizontalSelectionScrollbar.value = 0;
    }

    private void Start()
    {
        seekBar.onValueChanged.AddListener(OnSeekBarValueChanged);
        ConfigManager.instance.currentVolume += OnVolumeChange;

        ChangeLanguage();
    }



    private void Update()
    {
        if (sliderReset)
        {
            slider.value = 1;
        }


        if (!isDragging && vidPlayer.isPlaying && vidPlayer.length > 0)
        {
            seekBar.value = (float)(vidPlayer.time / vidPlayer.length);
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

    private void PlayHoldingScreenVideo()
    {
        holdingVideoPlayer.url = Application.streamingAssetsPath + folderName + "/Holding.mp4";
        holdingVideoPlayer.Play();
    }

    private void StopHoldingVideoPlayer()
    {
        holdingVideoPlayer.Stop();
    }

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
  
    private IEnumerator LoadVideoSelectionSprite()
    {
        yield return null;

        string path = Application.streamingAssetsPath + folderName + "/Images/SelectedImages/";
        int index = 1; 

        while (true)
        {
            string filePath = Path.Combine(path, $"{index}.png");

            if (File.Exists(filePath))
            {
                var rawData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(rawData);
                videoSelectionSprites.Add(tex);
            }
            else
            {
                break; 
            }

            index++; 
            yield return null; 
        }
    }

    private IEnumerator LoadRoundImages()
    {
        yield return null;

        string path = Application.streamingAssetsPath + folderName + "/Images/RoundImages/";
        int index = 1;

        while (true)
        {
            string filePath = Path.Combine(path, $"{index}.png");

            if (File.Exists(filePath))
            {
                var rawData = File.ReadAllBytes(filePath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(rawData);
                videoRoundTexture.Add(tex);
            }
            else
            {
                break;
            }

            index++;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < videoRoundTexture.Count; i++)
        {
            horizontalButtonsParentTransfom.GetChild(i).GetChild(1).GetComponent<RawImage>().texture = videoRoundTexture[i];
            buttonsParentTransform.GetChild(i).GetChild(1).GetComponent<RawImage>().texture = videoRoundTexture[i];
        }
    }
    public void SelectVideoImage(int index)
    {
        if (!isTransition)
        {
            horizontalSelectionScrollbar.value = Remap(scrollBar.value,1,0,0,1);
        }
        

        foreach (Transform child in buttonsParentTransform)
        {
            child.GetChild(0).gameObject.SetActive(false);
            child.GetChild(2).GetComponent<TextMeshProUGUI>().color = nonSelectedColor;
        }

        foreach (Transform child in horizontalButtonsParentTransfom)
        {
            child.GetChild(0).gameObject.SetActive(false);
            child.GetChild(2).GetComponent<TextMeshProUGUI>().color = nonSelectedColor;
           
        }

        horizontalButtonsParentTransfom.GetChild(index).GetChild(0).gameObject.SetActive(true);
        horizontalButtonsParentTransfom.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;


        buttonsParentTransform.GetChild(index).GetChild(0).gameObject.SetActive(true);
        buttonsParentTransform.GetChild(index).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;

        SelectText(index);

        if (index >= 0 && index < videoSelectionSprites.Count)
        {
            videoSelectImage.texture = videoSelectionSprites[index];
        }

        videoIndex = index;

        foreach (GameObject i in videoPlayAndPauseButtons)
        {
            i.SetActive(infoText.infos[index].isVideo);
        }

        if (!isTransition)
        {
            Invoke(nameof(SwitchToVideoScreen), 0.5f);
        }
      

    }
    private void SelectText(int index)
    {
        // For English
        
            bigTextHolder.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].date;
            bigTextHolder.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info2;
            bigTextHolder.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info3;
            bigTextHolder.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info4;
            bigTextHolder.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info5;

            smallTextHolder.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].date;
            smallTextHolder.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info2;
            smallTextHolder.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info3;
            smallTextHolder.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info4;
            smallTextHolder.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].language[0].info5;

        // For Hindi

      
            bigTextHolder.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].date;
            bigTextHolder.GetChild(1).GetChild(1).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info2, AppLanguage.Hindi);
            bigTextHolder.GetChild(1).GetChild(2).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info3, AppLanguage.Hindi);
            bigTextHolder.GetChild(1).GetChild(3).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info4, AppLanguage.Hindi);
            bigTextHolder.GetChild(1).GetChild(4).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info5, AppLanguage.Hindi);

            smallTextHolder.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText.infos[index].date;
            smallTextHolder.GetChild(1).GetChild(1).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info2, AppLanguage.Hindi);
            smallTextHolder.GetChild(1).GetChild(2).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info3, AppLanguage.Hindi);
            smallTextHolder.GetChild(1).GetChild(3).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info4, AppLanguage.Hindi);
            smallTextHolder.GetChild(1).GetChild(4).GetComponent<TmpTextLanguageManager>().SetContent(infoText.infos[index].language[1].info5, AppLanguage.Hindi);
    }
      
        
    private void SwitchToVideoScreen()
    {
        _PageTransition3.Transition();
    }
    private void SwitchToSelectionScreen()
    {
        if (!checker.activeSelf)
        {
           _PageTransition4_BackFromVideo.Transition();
        }

        videoButton.sprite = videoButtonSprite[1];

    }
    private void PlayVideo(string videoName)
    {
        if (tempName != videoName)
        {
            vidPlayer.url = Application.streamingAssetsPath + folderName + "/videos/" + videoName + ".mp4";

            vidPlayer.Prepare();

            vidPlayer.prepareCompleted += (VideoPlayer vp) =>
            {
                seekBar.maxValue = 1;
                seekBar.value = 0;
                vidPlayer.Play();

            };
        }
        else
        {
            vidPlayer.Play();
        }



        if (!animationState)
        {
            animationState = true;
            PlayAnimator(animationState);

            foreach (CanvasGroup i in VideoPlayerScreenCanvas)
            {
                StartCoroutine(FadeCanvas(i, 1, 0, 1));
            }

            StartCoroutine(FadeCanvas(seekBarCanavasGroup, 0, 1, 1));

            VideoPlayerScreenCanvas[1].blocksRaycasts = false;
            seekBarCanavasGroup.blocksRaycasts = true;
        }
        else
        {
            videoButton.sprite = videoButtonSprite[1];
        }

        tempName = videoName;

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

            foreach (CanvasGroup i in VideoPlayerScreenCanvas)
            {
                StartCoroutine(FadeCanvas(i, 0, 1, 1));
            }

            StartCoroutine(FadeCanvas(seekBarCanavasGroup, 1, 0, 1));
            VideoPlayerScreenCanvas[1].blocksRaycasts = true;
            seekBarCanavasGroup.blocksRaycasts = false;
        }

       
        videoButton.sprite = videoButtonSprite[1];
        tempName = "tempName";
        videoStatus = false;

      
    }
    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        StopVideo();
    }
    public void Home()
    {
        _PageTransition4_ToHome.Transition();
        PlayHoldingScreenVideo();
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
            PlayVideo(infoText.infos[videoIndex].videoname);
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
            Invoke(nameof(SwitchToSelectionScreen), 0.8f);
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

    private IEnumerator FadeCanvas(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }

    bool isDragging;

    public void OnSeekBarValueChanged(float value)
    {
        if (vidPlayer.isPrepared)
        {
            double newTime = vidPlayer.length * value;
            vidPlayer.time = newTime;
        }
    }

    public void OnBeginDragSeekBar()
    {
        isDragging = true;
        Debug.Log("onDrag");
        vidPlayer.Pause();
    }

    public void OnEndDragSeekBar()
    {
        isDragging = false;
        Debug.Log("onDragENd");
        vidPlayer.Play();
    }

    public void TransitionValue(bool value)
    {
        isTransition = value;
    }

    private void OnLanguageChanged(int selectedIndex)
    {
        if (lanugaeDropdown == null || lanugaeDropdown.Length == 0) return;

       
        foreach (var dropdown in lanugaeDropdown)
        {
            dropdown.SetValueWithoutNotify(selectedIndex); 
        }

      
        ApplyLanguage(selectedIndex);
    }

    private void ApplyLanguage(int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                currentLanguage = AppLanguage.English;
                break;
            case 1:
                currentLanguage = AppLanguage.Hindi;
                break;
            default:
                Debug.LogWarning("Selected language not handled.");
                break;
        }

        ChangeLanguage();

    }

    private void ChangeLanguage()
    {
        for (int i = 0; i < buttonsParentTransform.childCount; i++)
        {
            buttonsParentTransform.GetChild(i).GetChild(2).GetComponent<TmpTextLanguageManager>().SetContent(
            infoText.infos[i].language[(int)currentLanguage].name, currentLanguage);

            horizontalButtonsParentTransfom.GetChild(i).GetChild(2).GetComponent<TmpTextLanguageManager>().SetContent(
            infoText.infos[i].language[(int)currentLanguage].name, currentLanguage);
        }

        bool showEnglish = currentLanguage == AppLanguage.English;
        foreach (GameObject obj in EnglishObjects)
        {
            obj.SetActive(showEnglish);
        }

        bool showHindi = currentLanguage == AppLanguage.Hindi;
        foreach (GameObject obj in HindiObjects)
        {
            obj.SetActive(showHindi);
        }

        if (currentLanguage == AppLanguage.English)
        {
            for (int i = 0; i < screenImages.Length; i++)
            {
                screenImages[i].sprite = englishSprites[i];
            }
        }
        else if (currentLanguage == AppLanguage.Hindi)
        {
            for (int i = 0; i < screenImages.Length; i++)
            {
                screenImages[i].sprite = hindiSprites[i];
            }
        }
    }
}
