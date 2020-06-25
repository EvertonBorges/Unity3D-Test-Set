using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Static obstacles in the middle of the line")]
    private List<GameObject> obstaclesPrefabInCenter;

    [SerializeField]
    [Tooltip("Obstacles that alternate between lines")]
    private List<GameObject> obstaclesPrefabAnywhere;

    [SerializeField]
    private GameObject coinPrefab;

    [SerializeField]
    [Range(9f, 15f)]
    [Tooltip("Minimal distance between obstacles")]
    private float minDistanceBetweenObstacles;

    [SerializeField]
    [Range(15f, 23f)]
    [Tooltip("Maximum distance between obstacles")]
    private float maxDistanceBetweenObstacles;

    // All prefab obstacles
    private List<GameObject> obstaclesPrefab = new List<GameObject>();

    // All instantiates obstacles
    private List<GameObject> obstacles = new List<GameObject>();
    // All instantiates coins
    private List<GameObject> coins = new List<GameObject>();
    // All positions start/end between obstacles in track
    private List<int[]> spacesBetweenObstacles = new List<int[]>();

    private List<GameObject> poolCoins = new List<GameObject>();

    void Start()
    {
        obstaclesPrefab.AddRange(obstaclesPrefabAnywhere);
        obstaclesPrefab.AddRange(obstaclesPrefabInCenter);

        // Instantiate all coins and storage in a list.
        for (int i = 0; i < 200; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.SetActive(false);
            poolCoins.Add(coin);
        }

        PositionateObstacles();
        PositionateCoins();
    }

    private void PositionateObstacles()
    {
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();
        spacesBetweenObstacles.Clear();

        float spawnZPosition = transform.position.z;
        float zPosition = spawnZPosition;

        while(zPosition < transform.position.z + 180f)
        {
            int sortPrefabIndex = Random.Range(0, obstaclesPrefab.Count);
            GameObject prefab = obstaclesPrefab[sortPrefabIndex];

            int[] spaceBetweenObstacles = new int[2];
            spaceBetweenObstacles[0] = ((int) Mathf.Ceil(zPosition)) + 1;

            zPosition += 
                obstacles.Count == 0 ? 
                Mathf.Ceil(maxDistanceBetweenObstacles) :
                Mathf.Ceil(Random.Range(minDistanceBetweenObstacles, maxDistanceBetweenObstacles));

            spaceBetweenObstacles[1] = ((int) zPosition) - 1;

            if (zPosition < transform.position.z + 180f)
            {
                GameObject obstacle = Instantiate(prefab, transform);

                int xPosition = obstaclesPrefabInCenter.Contains(prefab) ? 0 : Random.Range(-1, 2);

                Vector3 newPosition = new Vector3(xPosition, 0f, zPosition);
                obstacle.transform.position = newPosition;
                obstacles.Add(obstacle);
                if (obstacles.Count > 1) spacesBetweenObstacles.Add(spaceBetweenObstacles);
            }
        }
    }

    private void PositionateCoins()
    {
        foreach(GameObject coin in coins)
        {
            coin.SetActive(false);
            poolCoins.Add(coin);
        }
        coins.Clear();

        for (int i = 0; i < spacesBetweenObstacles.Count; i++)
        {
            bool spawnCoins = Random.Range(0, 100) < 75 ? true : false;
            if (spawnCoins)
            {
                int sortXPosition = Random.Range(-1, 2);
                for (int z = spacesBetweenObstacles[i][0]; z <= spacesBetweenObstacles[i][1]; z++)
                {
                    GameObject coin = poolCoins[0];
                    coin.transform.position = new Vector3(sortXPosition, 0.5f, z);
                    coin.SetActive(true);
                    coins.Add(coin);
                    poolCoins.RemoveAt(0);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When game need to increase the player speed
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().IncreaseSpeed();
            transform.position += Vector3.forward * 180f * 2;
            PositionateObstacles();
            PositionateCoins();
        }
    }

}
