using UnityEngine;

public class VirusClearance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the clearance zone: " + other.name);

        VirusNPC virus = other.GetComponent<VirusNPC>();

        if (virus != null)
        {
            Debug.Log("Virus detected! Destroying: " + other.name);
            Destroy(other.gameObject);
        }
    }
}