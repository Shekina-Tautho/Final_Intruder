
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimulationUIController : MonoBehaviour
{
    [Header("Stage Information")]
    public TMP_Text stageTitle;
    public TMP_Text stageDescription;
    public TMP_Text stageNumber;

    [Header("Stage Images - Normal")]
    public Image exposureNormal;
    public Image tractEntryNormal;
    public Image encounterNormal;
    public Image infectionNormal;
    public Image replicationNormal;
    public Image releaseNormal;
    public Image spreadNormal;
    public Image immuneResponseNormal;
    public Image inflammationNormal;
    public Image outcomeNormal;

    [Header("Stage Images - Glow")]
    public Image exposureGlow;
    public Image tractEntryGlow;
    public Image encounterGlow;
    public Image infectionGlow;
    public Image replicationGlow;
    public Image releaseGlow;
    public Image spreadGlow;
    public Image immuneResponseGlow;
    public Image inflammationGlow;
    public Image outcomeGlow;

    [Header("Stage Descriptions")]
    [TextArea(3, 5)] public string exposureDescription;
    [TextArea(3, 5)] public string tractEntryDescription;
    [TextArea(3, 5)] public string encounterDescription;
    [TextArea(3, 5)] public string infectionDescription;
    [TextArea(3, 5)] public string replicationDescription;
    [TextArea(3, 5)] public string releaseDescription;
    [TextArea(3, 5)] public string spreadDescription;
    [TextArea(3, 5)] public string immuneResponseDescription;
    [TextArea(3, 5)] public string inflammationDescription;
    [TextArea(3, 5)] public string outcomeDescription;

    [Header("Starting Stage")]
    [Range(1, 10)]
    public int startingStage = 1;

    private int currentStage;

    private Image[] normalImages;
    private Image[] glowImages;

    private string[] stageTitles;
    private string[] stageDescriptions;

    private void Awake()
    {
        InitializeStageData();
        SetStage(startingStage);
    }

    private void InitializeStageData()
    {
        normalImages = new Image[]
        {
            exposureNormal,
            tractEntryNormal,
            encounterNormal,
            infectionNormal,
            replicationNormal,
            releaseNormal,
            spreadNormal,
            immuneResponseNormal,
            inflammationNormal,
            outcomeNormal
        };

        glowImages = new Image[]
        {
            exposureGlow,
            tractEntryGlow,
            encounterGlow,
            infectionGlow,
            replicationGlow,
            releaseGlow,
            spreadGlow,
            immuneResponseGlow,
            inflammationGlow,
            outcomeGlow
        };

        stageTitles = new string[]
        {
            "EXPOSURE",
            "TRACT ENTRY",
            "CELL ENCOUNTER",
            "INFECTION",
            "REPLICATION",
            "RELEASE",
            "SPREAD",
            "IMMUNE RESPONSE",
            "INFLAMMATION",
            "OUTCOME"
        };

        stageDescriptions = new string[]
        {
            exposureDescription,
            tractEntryDescription,
            encounterDescription,
            infectionDescription,
            replicationDescription,
            releaseDescription,
            spreadDescription,
            immuneResponseDescription,
            inflammationDescription,
            outcomeDescription
        };
    }

    public void SetStage(int stage)
    {
        if (stage < 1 || stage > 10)
        {
            Debug.LogWarning("Invalid stage number. Choose a stage from 1 to 10.");
            return;
        }

        currentStage = stage;

        for (int i = 0; i < normalImages.Length; i++)
        {
            if (normalImages[i] != null)
                normalImages[i].gameObject.SetActive(i != stage - 1);

            if (glowImages[i] != null)
                glowImages[i].gameObject.SetActive(i == stage - 1);
        }

        if (stageTitle != null)
            stageTitle.text = stageTitles[stage - 1];

        if (stageDescription != null)
            stageDescription.text = stageDescriptions[stage - 1];

        if (stageNumber != null)
            stageNumber.text = stage.ToString("D2") + " / 10";
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }
}
