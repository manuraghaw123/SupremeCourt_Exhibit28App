﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Windows;


public static class TextExtension // created extension for Text and TextMeshPro
{
    public static void SetHindiText(this Text text, string value)
    {
        Font krutiDev = Resources.Load("CD_Kruti_Dev_010") as Font;
        text.font = krutiDev;
        text.text = HindiCorrector.GetCorrectedHindiText(value);
    }

    public static void SetHindiTMPro(this TMP_Text text, string value)
    {
        Regex reg = new Regex("<font=\"(.*?)\">(.*?)</font>",RegexOptions.Singleline);
        List<string> exceptionals = new List<string>();
        int frequency = -1;
        Match match = reg.Match(value);
        while (match.ToString() != "")
        {
            frequency++;
            exceptionals.Add(match.ToString());
            value = HindiCorrector.ReplaceFirstOccurrence(value, match.ToString(), "क्ष्" + frequency + "द्व");
            match = reg.Match(value);
        }
        TMP_FontAsset krutiDev = Resources.Load("CD_Kruti_Dev_010 SDF") as TMP_FontAsset;
        text.font = krutiDev;
        value = HindiCorrector.GetCorrectedHindiText(value);

        //value = string.Format(value, exceptionals.ToArray());
        value = value.Replace("M़", "M+");
        value = value.Replace("<f़", "<+f");
        value = value.Replace("<़", "<+");
        value = value.Replace("Mf़", "fM+");
        text.text = value;
    }
}
