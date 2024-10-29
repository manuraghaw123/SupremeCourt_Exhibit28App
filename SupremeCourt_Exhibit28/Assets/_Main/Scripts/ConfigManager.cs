using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class ConfigJson
{
    public bool isEnglish;
    public float volume;
    public bool isPrimaryContent;
}

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager instance;

    public Action<bool> currentLanguage;
    public Action<float> currentVolume;
    public Action<bool> primaryContent;
    public Action configLoaded;


    public ConfigJson config;

    private float volume;
    private bool isPrimaryContent;

    private void Awake()
    {
        instance = this;

        string json = File.ReadAllText(Application.streamingAssetsPath + "/ConfigJson.json");
        config = JsonUtility.FromJson<ConfigJson>(json);
        isPrimaryContent = config.isPrimaryContent;
        volume = config.volume;
    }
    private void Start()
    {
        Invoke(nameof(LoadConfig), 0.2f);
    }
    private void SaveJson()
    {
        string data = JsonUtility.ToJson(config);
        File.WriteAllText(Application.streamingAssetsPath + "/ConfigJson.json", data);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            VolumeFun(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            VolumeFun(-0.1f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            LanguageSelection(true);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            LanguageSelection(false);
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            isPrimaryContent = !isPrimaryContent;
            PrimaryContentSelection(isPrimaryContent);

        }
    }
    private void LoadConfig()
    {
        currentLanguage?.Invoke(config.isEnglish);
        currentVolume?.Invoke(config.volume);
        primaryContent?.Invoke(config.isPrimaryContent);
        configLoaded?.Invoke();
    }
    private void VolumeFun(float value)
    {
        volume = Mathf.Clamp(volume + value, 0f, 1f);
        currentVolume?.Invoke(volume);
        config.volume = volume;
        SaveJson();
    }
    private void LanguageSelection(bool value)
    {
        config.isEnglish = value;
        currentLanguage?.Invoke(value);
        SaveJson();
    }
    private void PrimaryContentSelection(bool value)
    {
        config.isPrimaryContent = value;
        primaryContent?.Invoke(value);
        SaveJson();
    }




}
