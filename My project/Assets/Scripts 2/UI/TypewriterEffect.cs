
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
        // Hide all text initially
        foreach (TMP_Text textObject in textObjects)
        {
            textObject.maxVisibleCharacters = 0;
        }

        // Hide Continue button
        if (continueButton != null)
            continueButton.SetActive(false);

        // Type each text object in order
        foreach (TMP_Text textObject in textObjects)
        {
            if (textObject == null)
                continue;

            textObject.ForceMeshUpdate();

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

        // Show Continue button after all text finishes
        if (continueButton != null)
            continueButton.SetActive(true);
    }
}
