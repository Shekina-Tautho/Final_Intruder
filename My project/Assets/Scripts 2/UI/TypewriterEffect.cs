
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Introduction Texts")]
    public TMP_Text[] textObjects;

    [Header("Typing Settings")]
    public float charactersPerSecond = 30f;
    public float delayBetweenTexts = 0.5f;

    [Header("Scene Transition")]
    public string nextSceneName = "ExposureScene";
    public float delayBeforeTransition = 1.5f;

    private void Start()
    {
        StartCoroutine(PlayIntroduction());
    }

    private IEnumerator PlayIntroduction()
    {
        // Hide all text initially
        foreach (TMP_Text textObject in textObjects)
        {
            if (textObject == null)
                continue;

            textObject.ForceMeshUpdate();
            textObject.maxVisibleCharacters = 0;
        }

        // Type each text one after another
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

        // Wait briefly after the final text
        yield return new WaitForSeconds(delayBeforeTransition);

        // Automatically load ExposureScene
        SceneManager.LoadScene(nextSceneName);
    }
}
