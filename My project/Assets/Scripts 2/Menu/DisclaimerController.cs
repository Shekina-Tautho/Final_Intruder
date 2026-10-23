using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisclaimerController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject disclaimerPanel;
    public Toggle disclaimerToggle;
    public Button continueButton;
    public Image continueImage;
    public Button playButton;
    public GameObject background;
    public GameObject quitButton;

    [Header("Continue Button Colors")]
    public Color disabledColor = Color.gray;
    private Color originalColor;

    private void Awake()
    {
        if (continueImage != null)
            originalColor = continueImage.color;

        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);

        if (continueButton != null)
            continueButton.interactable = false;

        if (disclaimerToggle != null)
            disclaimerToggle.isOn = false;

        UpdateContinueButton(false);
    }

    private void OnEnable()
    {
        if (disclaimerToggle != null)
            disclaimerToggle.onValueChanged.AddListener(UpdateContinueButton);
    }

    private void OnDisable()
    {
        if (disclaimerToggle != null)
            disclaimerToggle.onValueChanged.RemoveListener(UpdateContinueButton);
    }

    public void OpenDisclaimer()
    {
        if (disclaimerToggle != null)
            disclaimerToggle.isOn = false;

        UpdateContinueButton(false);

        if (background != null)
            background.SetActive(false);

        if (playButton != null)
            playButton.gameObject.SetActive(false);

        if (quitButton != null)
            quitButton.SetActive(false);

        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(true);
    }

    public void UpdateContinueButton(bool isChecked)
    {
        if (continueButton != null)
            continueButton.interactable = isChecked;

        if (continueImage != null)
            continueImage.color = isChecked ? originalColor : disabledColor;
    }

    public void ContinueToSimulation()
    {
        if (disclaimerToggle == null || !disclaimerToggle.isOn)
            return;

        SceneManager.LoadScene("SampleScene");
    }

    public void CloseDisclaimer()
    {
        if (disclaimerPanel != null)
            disclaimerPanel.SetActive(false);

        if (background != null)
            background.SetActive(true);

        if (playButton != null)
            playButton.gameObject.SetActive(true);

        if (quitButton != null)
            quitButton.SetActive(true);
    }
}