
using UnityEngine;
using System.Collections;

public class SceneStartVoiceOver : MonoBehaviour
{
    [Header("Voice-Over Audio")]
    public AudioSource voiceOverAudioSource;

    [Header("Voice-Over Recordings")]
    public AudioClip firstVoiceOverClip;
    public AudioClip secondVoiceOverClip;

    [Header("Simulation UI")]
    public SimulationUIController simulationUIController;

    [Header("Delay Settings")]
    public float startDelay = 1f;
    public float secondVoiceOverDelay = 2f;

    private IEnumerator Start()
    {
        // Wait 1 second after the scene starts
        yield return new WaitForSeconds(startDelay);

        if (voiceOverAudioSource == null)
        {
            Debug.LogWarning("Voice-over AudioSource is missing!");
            yield break;
        }

        // Play the first recording
        if (firstVoiceOverClip != null)
        {
            voiceOverAudioSource.PlayOneShot(firstVoiceOverClip);

            // Wait until the first recording finishes
            yield return new WaitForSeconds(firstVoiceOverClip.length);
        }
        else
        {
            Debug.LogWarning("First voice-over clip is missing!");
            yield break;
        }

        // Calculate when to change the simulation stage
        float transitionLeadTime = Mathf.Min(1f, secondVoiceOverDelay);

        // Wait until 1 second before the second voice-over
        yield return new WaitForSeconds(
            secondVoiceOverDelay - transitionLeadTime
        );

        // Change the simulation panel to Stage 7
        if (simulationUIController != null)
        {
            simulationUIController.SetStage(7);
        }
        else
        {
            Debug.LogWarning("Simulation UI Controller is missing!");
        }

        // Wait 1 second before playing the second recording
        yield return new WaitForSeconds(transitionLeadTime);

        // Play the second recording
        if (secondVoiceOverClip != null)
        {
            voiceOverAudioSource.PlayOneShot(secondVoiceOverClip);
        }
        else
        {
            Debug.LogWarning("Second voice-over clip is missing!");
        }
    }
}
