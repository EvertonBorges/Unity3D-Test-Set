using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameController))]
public class LevelManager : MonoBehaviour
{

    private static LevelManager _instance;

    private GameController _gameController;
    private int coins = 0;

    void Awake()
    {
        _gameController = GetComponent<GameController>();
    }

    public static LevelManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType<LevelManager>();
        }

        return _instance;
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