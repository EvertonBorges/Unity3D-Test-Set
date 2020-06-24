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

    private PlayerDatas datas = null;

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

    private void SaveDatas()
    {
        if (datas != null) UpdateDatas(); else CreateDatas();

        _gameController.Save(datas);
    }

    private void UpdateDatas()
    {
        if (datas.distance < distance)
        {
            datas.distance = distance;
        }
        datas.coins += coins;

        int position = -1;
        int[] scores = new int[10];
        for (int i = 0; i < datas.scores.Length; i++)
        {
            int score = datas.scores[i];
            scores[i] = score;
            if (this.score > score)
            {
                position = i;
                scores[i] = this.score;
                break;
            }
        }
        if (position != -1)
        {
            for (int i = position + 1; i < datas.scores.Length - 1; i++)
            {
                scores[i] = datas.scores[i - 1];
            }
        }

        datas.scores = scores;
    }

    private void CreateDatas()
    {
        int[] scores = new int[10];
        scores[0] = this.score;

        datas = new PlayerDatas();
        datas.distance = distance;
        datas.coins = coins;
        datas.scores = scores;
    }

    public void LoadDatas(PlayerDatas datas)
    {
        int bestScore;
        bool isNewRecord;

        if (datas != null)
        {
            this.datas = datas;
            bestScore = this.score > datas.scores[0] ? this.score : datas.scores[0];
            isNewRecord = this.score > datas.scores[0];
        } else
        {
            bestScore = score;
            isNewRecord = true;
        }

        _gameController.UpdateBestScore(bestScore.ToString(), isNewRecord);

        SaveDatas();
    }

}