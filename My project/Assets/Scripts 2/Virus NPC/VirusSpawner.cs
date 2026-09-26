using UnityEngine;

public class VirusSpawner : MonoBehaviour
{
    public GameObject virusPrefab;
    public BoxCollider spawnZone;
    public int virusCount = 20;

    void Start()
    {
        for (int i = 0; i < virusCount; i++)
        {
            SpawnVirus();
        }
    }

    void SpawnVirus()
    {
        Vector3 spawnPosition = GetRandomPositionInZone();

        Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    Vector3 GetRandomPositionInZone()
    {
        Bounds bounds = spawnZone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }
}