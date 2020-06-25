using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

[Serializable]
public class PlayerDatas
{
    public int coins { set; get; }
    public int distance { set; get; }
    public int[] scores { set; get; }
}

[RequireComponent(typeof(SceneController))]
public class GameController : MonoBehaviour {

    [SerializeField]
    private SceneController sceneController;

    [SerializeField]
    private Button buttonPause;

    [SerializeField]
    private Image panelPause;

    [SerializeField]
    private Image panelGameOver;

    [SerializeField]
    private Image panelCoins;

    [SerializeField]
    private Image panelScore;

    [SerializeField]
    private Image panelDistance;

    [SerializeField]
    private Text textCoins;

    [SerializeField]
    private Text textScore;

    [SerializeField]
    private Text textScoreMultiplier;

    [SerializeField]
    private Text textDistance;

    [SerializeField]
    private Text textBestScore;

    [SerializeField]
    private Text textBestScoreNew;

    [SerializeField]
    private AiController aiController;

    [SerializeField]
    private Text labelAiIsPlaying;

    private PlayerController _playerController;
    private LevelManager _levelManager;
    private Settings _settings;

    private string _filePath;

    void Awake()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _levelManager = GetComponent<LevelManager>();
        _settings = ConfigurationsSingleton.Instance.settings;

        PlayerController.UpdatePlayerController(!_settings.aiEnable);
        aiController.gameObject.SetActive(_settings.aiEnable);
        AiController.SetFocusCoin(_settings.aiCoinsEnable);
        labelAiIsPlaying.gameObject.SetActive(_settings.aiEnable);

        _filePath = Application.persistentDataPath + "/playerInfo.dat";
    }

    void Start() 
    {
        buttonPause.gameObject.SetActive(false);

        panelPause.gameObject.SetActive(false);
        panelGameOver.gameObject.SetActive(false);

        panelCoins.gameObject.SetActive(false);
        panelScore.gameObject.SetActive(false);
        panelDistance.gameObject.SetActive(false);
    }

    public void Pause() 
    {
        if (!panelPause.gameObject.activeSelf)
        {
            _playerController.Pause();
            sceneController.PressButton();
            panelPause.gameObject.SetActive(true);
            labelAiIsPlaying.gameObject.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (!panelGameOver.gameObject.activeSelf)
        {
            PlayerDatas datas = null;
            if (File.Exists(_filePath))
            {
                datas = Load();
            }
            _levelManager.LoadDatas(datas);

            panelGameOver.gameObject.SetActive(true);
        }
    }

    public void UpdateCoins(string coins)
    {
        textCoins.text = coins;
    }

    public void UpdateScore(string score, string multiplier)
    {
        textScore.text = score;
        textScoreMultiplier.text = multiplier;
    }

    public void UpdateDistance(string distance)
    {
        textDistance.text = distance;
    }

    public void UpdateBestScore(string bestScore, bool isNewRecord)
    {
        textBestScore.text = bestScore;
        textBestScoreNew.gameObject.SetActive(isNewRecord);
    }

    public void ShowUiGame()
    {
        panelCoins.gameObject.SetActive(true);
        panelScore.gameObject.SetActive(true);
        panelDistance.gameObject.SetActive(true);
    }

    public void ShowPauseButton()
    {
        buttonPause.gameObject.SetActive(true);
    }

    public void UnPause()
    {
        _playerController.UnPause();
        sceneController.PressButton();
        panelPause.gameObject.SetActive(false);
        labelAiIsPlaying.gameObject.SetActive(_settings.aiEnable);
    }

    public void Save(PlayerDatas datas)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(_filePath);

        binaryFormatter.Serialize(file, datas);
        file.Close();
    }

    private PlayerDatas Load()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(_filePath, FileMode.Open);

        PlayerDatas datas = (PlayerDatas) binaryFormatter.Deserialize(file);
        file.Close();

        int bestScore = datas.scores[datas.scores.Length - 1];
        textBestScore.text = bestScore.ToString();

        return datas;
    }

}