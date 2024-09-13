using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private SpawnPoint[] spawnPoints;

    private Dictionary<string, GameObject[]> poolTable;

    private void Awake()
    {
        poolTable = new();
    }

    private void Start()
    {
        Debug.Assert(spawnPoints != null);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject poolParent = new(spawnPoints[i].EnemyPrefab.name);

            GameObject[] pool = CreateObjectPool(poolParent, spawnPoints[i]);
            poolTable.Add(spawnPoints[i].name, pool);
        }

        string[] poolTableKeys = new string[spawnPoints.Length];

        for (int i = 0; i < poolTableKeys.Length; i++)
        {
            poolTableKeys[i] = spawnPoints[i].name;
        }

        StartCoroutine(Coroutine_StartSpawn(poolTableKeys));
    }

    private GameObject[] CreateObjectPool(GameObject parent, SpawnPoint spawnPoint)
    {
        GameObject[] result = new GameObject[spawnPoint.SpawnCount];

        for (int i = 0; i < spawnPoint.SpawnCount; i++)
        {
            result[i] = Instantiate(spawnPoint.EnemyPrefab, parent.transform, false);
            result[i].name = $"{spawnPoint.EnemyPrefab.name}_{i:000}";

            Vector3 position = new()
            {
                x = spawnPoint.SpawnPoints[i].x,
                z = spawnPoint.SpawnPoints[i].y
            };

            result[i].transform.localPosition = position;
            result[i].SetActive(false);
        }

        return result;
    }

    private IEnumerator Coroutine_StartSpawn(string[] poolTableKeys)
    {
        WaitForSeconds wait = new(0.01f);

        while (true)
        {
            int random = UnityEngine.Random.Range(0, poolTableKeys.Length);

            Spawn(poolTableKeys[random]);

            yield return wait;
        }
    }

    private void Spawn(string poolTableKey)
    {
        GameObject[] pool = poolTable[poolTableKey];
        GameObject spawnObject = null;

        foreach (GameObject go in pool)
        {
            if (go.activeSelf == false)
            {
                spawnObject = go;
                break;
            }
        }

        if (spawnObject == null)
            return;

        spawnObject.SetActive(true);
    }
}
