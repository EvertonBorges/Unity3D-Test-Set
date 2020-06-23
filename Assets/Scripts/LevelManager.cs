using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;

    private GameController _gameController;
    private PlayerController _player;

    private int coins = 0;
    private int distance = 0;

    private int[] _distanceTravels = { 0, 0, 0, 0, 0 };
    private int score = 0;

    void Awake()
    {
        _gameController = GetComponent<GameController>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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

        int speedMultiplier = _player.SpeedMultiplication();
        _distanceTravels[speedMultiplier - 1] = distance;
        score = 0;
        int _distancePast = 0;
        for (int i = 0; i < speedMultiplier; i++)
        {
            int scoreByDistance = (_distanceTravels[i] - _distancePast) * (i + 1);
            score += scoreByDistance;
            _distancePast = _distanceTravels[i];
        }

        _gameController.UpdateDistance(distance.ToString());
        _gameController.UpdateScore(score.ToString(), speedMultiplier.ToString());
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