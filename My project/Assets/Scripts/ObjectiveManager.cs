using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject objectivePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    private bool isPanelActive = false;

    void Update()
    {
        if (isPanelActive)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                HidePanel();
            }
        }
    }

    public void ShowIntroObjective()
    {
        isPanelActive = true;
        objectivePanel.SetActive(true);

        titleText.text = "Mission Start: Influenza Virus";

        bodyText.text =
        "You are an Influenza Virus entering a human host.\n\n" +
        "Primary Objective:\n- Infect target cells to increase infection count\n\n" +
        "Threat:\n- Macrophages patrol and eliminate viruses\n\n" +
        "Goal: Spread before immune system destroys you.";
    }

    void HidePanel()
    {
        isPanelActive = false;
        objectivePanel.SetActive(false);
    }
}