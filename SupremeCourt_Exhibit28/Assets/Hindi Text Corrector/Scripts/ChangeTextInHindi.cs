using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChangeTextInHindi : MonoBehaviour
{
   

    void Start()
    {
       Text txt = GetComponent<Text>();
        TMP_Text tmpTxt = GetComponent<TMP_Text>();

        if(txt != null)
        {
            txt.SetHindiText(txt.text);
        }
        else if(tmpTxt != null)
        {
            tmpTxt.SetHindiTMPro(tmpTxt.text);
        }
    }

   

   
}
