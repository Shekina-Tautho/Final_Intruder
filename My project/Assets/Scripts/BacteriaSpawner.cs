using UnityEngine;

public class BacteriaSpawner : MonoBehaviour
{
    [Header("Bacteria Prefabs")]
    public GameObject[] bacteriaPrefabs;

    [Header("Spawn Area")]
    public BoxCollider spawnArea;

    [Header("Spawn Settings")]
    public int spawnCount = 50;

    [Header("Scale Variation")]
    public float minScale = 0.7f;
    public float maxScale = 1.3f;

    void Start()
    {
        SpawnBacteria();
    }

    void SpawnBacteria()
    {
        if (spawnArea == null) return;

        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            GameObject prefab = bacteriaPrefabs[
                Random.Range(0, bacteriaPrefabs.Length)
            ];

            GameObject obj = Instantiate(prefab, randomPos, Random.rotation);

            float scale = Random.Range(minScale, maxScale);
            obj.transform.localScale *= scale;
        }
    }
}