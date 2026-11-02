
using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Introduction Texts")]
    public TMP_Text[] textObjects;

    [Header("Typing Settings")]
    public float charactersPerSecond = 30f;
    public float delayBetweenTexts = 0.5f;

    [Header("Continue Button")]
    public GameObject continueButton;

    private void Start()
    {
        StartCoroutine(PlayIntroduction());
    }

    private IEnumerator PlayIntroduction()
    {
        // Hide the button at the beginning
        if (continueButton != null)
            continueButton.SetActive(false);

        // Prepare all text objects
        foreach (TMP_Text textObject in textObjects)
        {
            if (textObject == null)
                continue;

            textObject.ForceMeshUpdate();
            textObject.maxVisibleCharacters = 0;
        }

        // Type each text object in sequence
        foreach (TMP_Text textObject in textObjects)
        {
            if (textObject == null)
                continue;

            int totalCharacters = textObject.textInfo.characterCount;

            for (int i = 0; i < totalCharacters; i++)
            {
                textObject.maxVisibleCharacters = i + 1;

                yield return new WaitForSeconds(
                    1f / charactersPerSecond
                );
            }

            yield return new WaitForSeconds(delayBetweenTexts);
        }

        // Show button only when every text is finished
        if (continueButton != null)
            continueButton.SetActive(true);
    }
}
