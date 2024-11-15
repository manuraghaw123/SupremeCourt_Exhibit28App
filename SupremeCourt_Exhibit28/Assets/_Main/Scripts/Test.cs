using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Test : MonoBehaviour
{

    [SerializeField] VideoPlayer vidPlayer;
    [SerializeField] private Slider seekBar;

    private bool isDragging = false;

    void Start()
    {
        seekBar.onValueChanged.AddListener(OnSeekBarValueChanged);
        PlayVideo("6");
    }

    private void PlayVideo(string value)
    {
        vidPlayer.url = Application.streamingAssetsPath + "/President/videos/" + value + ".mp4";
        vidPlayer.Prepare();

        vidPlayer.prepareCompleted += (VideoPlayer vp) =>
        {
            seekBar.maxValue = 1;
            seekBar.value = 0;
            vidPlayer.Play();
        };
    }

  
    void Update()
    {
        if (!isDragging && vidPlayer.isPlaying && vidPlayer.length > 0)
        {
            seekBar.value = (float)(vidPlayer.time / vidPlayer.length);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            PlayVideo("7");
        }
    }

    private void OnSeekBarValueChanged(float value)
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
}
