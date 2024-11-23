using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum AppLanguage
{ 
 English,
 Hindi
}

[System.Serializable]
public class PerLanguageSettings
{
    public AppLanguage language;
    public float fontSize;
    public float lineSpacing;
    public float characterSpacing;
    public bool isBold;
}

public class TmpTextLanguageManager : MonoBehaviour
{
    [SerializeField]
    private List<PerLanguageSettings> languageSettings;

    private PerLanguageSettings originalSetting;

    private TMP_FontAsset originalFont;
    private TMP_Text tmp_text;

    private bool isInitialised;

    public void Initialise()
    {
        isInitialised = true;
        tmp_text = GetComponent<TMP_Text>();
        originalFont = tmp_text.font;
        originalSetting = new PerLanguageSettings()
        {
            fontSize = tmp_text.fontSize,
            lineSpacing = tmp_text.lineSpacing,
            characterSpacing = tmp_text.characterSpacing,
            isBold = tmp_text.fontStyle == FontStyles.Bold
        };
    }

    public void SetContent(string text, AppLanguage appLanguage)
    {
        if(!isInitialised)
            Initialise();

        switch (appLanguage)
        {
            case AppLanguage.English:
                tmp_text.text = text;
                tmp_text.font = originalFont;
                break;
            case AppLanguage.Hindi:
                tmp_text.SetHindiTMPro(text);
                break;
        }

        PerLanguageSettings languageSetting = languageSettings.Find(x => x.language == appLanguage);
        if (languageSetting != null)
        {
            if (languageSetting.fontSize != 0)
                tmp_text.fontSize = languageSetting.fontSize;

            if (languageSetting.lineSpacing != 0)
                tmp_text.lineSpacing = languageSetting.lineSpacing;

            if (languageSetting.characterSpacing != 0)
                tmp_text.characterSpacing = languageSetting.characterSpacing;

            tmp_text.fontStyle = languageSetting.isBold ? FontStyles.Bold : FontStyles.Normal;
        }
        else
        {
            tmp_text.fontSize = originalSetting.fontSize;
            tmp_text.lineSpacing = originalSetting.lineSpacing;
            tmp_text.characterSpacing = originalSetting.characterSpacing;
            tmp_text.fontStyle = originalSetting.isBold ? FontStyles.Bold : FontStyles.Normal;
        }
    }
}
