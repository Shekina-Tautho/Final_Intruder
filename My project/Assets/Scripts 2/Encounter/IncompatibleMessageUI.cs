
using UnityEngine;
using System.Collections;

public class IncompatibleMessageUI : MonoBehaviour
{
    [Header("Message Panel")]
    public GameObject messagePanel;

    [Header("Display Settings")]
    public float displayDuration = 2f;

    private Coroutine messageCoroutine;

    void Awake()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    public void ShowMessage()
    {
        if (messagePanel == null)
        {
            Debug.LogWarning("Message Panel is not assigned.");
            return;
        }

        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
        }

        messageCoroutine = StartCoroutine(DisplayMessage());
    }

    private IEnumerator DisplayMessage()
    {
        messagePanel.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        messagePanel.SetActive(false);

        messageCoroutine = null;
    }
}
