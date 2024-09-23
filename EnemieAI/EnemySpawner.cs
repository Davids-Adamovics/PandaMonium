using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies = 5;
    public Vector3 spawnAreaSize = new Vector3(10, 0, 10);
    public PostProcessVolume postProcessVolume; // Add this line

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            Vector3 spawnPosition = new Vector3(
                transform.position.x + Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                transform.position.y,
                transform.position.z + Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Assign the PostProcessVolume to the enemy
            Enemy_AI enemyAI = enemyInstance.GetComponent<Enemy_AI>();
            if (enemyAI != null)
            {
                enemyAI.postProcessVolume = postProcessVolume; // Assign the volume
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}
