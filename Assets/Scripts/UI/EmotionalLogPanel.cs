using UnityEngine;
using TMPro;
using System.Collections;

public class EmotionalLogPanel : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public CanvasGroup panelGroup;

    public void ShowMessage(string message)
    {
        messageText.text = message;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        panelGroup.alpha = 1;
        yield return new WaitForSeconds(2f);
        panelGroup.alpha = 0;
    }
}