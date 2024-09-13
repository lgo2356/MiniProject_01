using UnityEngine;

public class SpawnPoint : ScriptableObject
{
    public Vector2 MapSize = new(-10f, 8.5f);
    public GameObject EnemyPrefab;
    public int SpawnCount = 20;
    public Vector2[] SpawnPoints;
}
