using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;

    private GameController _gameController;
    private GameObject _player;

    private int coins = 0;
    private int distance = 0;
    private int score = 0;

    void Awake()
    {
        _gameController = GetComponent<GameController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public static LevelManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<LevelManager>();
        }

        return _instance;
    }

    void Update()
    {
        distance = (int) Mathf.Ceil(_player.transform.position.z);
        _gameController.UpdateDistance(distance.ToString());
    }

    public void Clear()
    {
        coins = 0;
    }

    public void AddCoin()
    {
        coins++;
        _gameController.UpdateCoins(coins.ToString());
    }
    
    public int GetCoins()
    {
        return coins;
    }

}