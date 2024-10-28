using UnityEngine;
using UnityEngine.UI;

public class VideoSelectionButton : MonoBehaviour
{
    private Button button;
    private int videoIndex;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        videoIndex = transform.GetSiblingIndex() + 1;
    }

}
