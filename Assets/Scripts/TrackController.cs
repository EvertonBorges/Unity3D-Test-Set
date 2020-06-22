using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] obstaclesPrefab;

    [SerializeField]
    [Range(9f, 15f)]
    private float minDistanceBetweenObstacles;

    [SerializeField]
    [Range(15f, 23f)]
    private float maxDistanceBetweenObstacles;

    private List<GameObject> obstacles = new List<GameObject>();

    void Start()
    {
        PositionateObstacles();
    }

    private void PositionateObstacles()
    {
        foreach(GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();

        float spawnZPosition = transform.position.z - 90f;
        float zPosition = spawnZPosition;

        while(zPosition < transform.position.z + 90f)
        {
            int sortPrefabIndex = Random.Range(0, obstaclesPrefab.Length);
            GameObject prefab = obstaclesPrefab[sortPrefabIndex];

            zPosition += 
                obstacles.Count == 0 ? 
                maxDistanceBetweenObstacles * 2 : 
                Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles);

            GameObject obstacle = Instantiate(prefab, transform);
            Vector3 newPosition = new Vector3(0f, 0f, zPosition);
            obstacle.transform.position = newPosition;
            obstacles.Add(obstacle);

            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position += Vector3.forward * 180f * 2;
            PositionateObstacles();
        }
    }

}
