using UnityEngine;
using UnityEngine.UI;

public class VideoSelectionButton : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
       int videoIndex = transform.GetSiblingIndex();
        Manager.instance.SelectVideoImage(videoIndex);
    }

}
