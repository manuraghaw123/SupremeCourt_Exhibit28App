using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLanguageText : MonoBehaviour
{
    [TextArea]
    private string englishTxt;
    [TextArea]
    private string hindiTxt;

    private TmpTextLanguageManager tmpTextLanguageManager;

    private void Reset()
    {
        tmpTextLanguageManager = GetComponent<TmpTextLanguageManager>();
    }

    private void Start()
    {
        tmpTextLanguageManager.SetContent(englishTxt, AppLanguage.English);
    }
}
