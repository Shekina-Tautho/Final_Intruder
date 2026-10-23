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

    [Header("Continue Button Colors")]
    public Color disabledColor = Color.gray;
    private Color originalColor;

    private void Awake()
    {
        originalColor = continueImage.color;

        disclaimerPanel.SetActive(false);
        continueButton.interactable = false;

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
        disclaimerToggle.isOn = false;
        UpdateContinueButton(false);
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
        if (!disclaimerToggle.isOn)
            return;

        SceneManager.LoadScene("SampleScene");
    }

    public void CloseDisclaimer()
    {
        disclaimerPanel.SetActive(false);
    }
}