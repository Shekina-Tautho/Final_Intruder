
using UnityEngine;

public class NPCReceptorAutoAttach : MonoBehaviour
{
    [Header("Automatic Receptor Detection")]
    public float checkInterval = 0.1f;

    private Virus virus;
    private ReceptorGroup[] receptorGroups;
    private float nextCheckTime;

    void Awake()
    {
        virus = GetComponent<Virus>();
    }

    void Start()
    {
        receptorGroups = FindObjectsByType<ReceptorGroup>(
            FindObjectsSortMode.None
        );
    }

    void Update()
    {
        if (virus == null || Time.time < nextCheckTime)
            return;

        nextCheckTime = Time.time + checkInterval;

        foreach (ReceptorGroup group in receptorGroups)
        {
            if (group != null)
            {
                group.TryAttach(virus);
            }
        }
    }
}
