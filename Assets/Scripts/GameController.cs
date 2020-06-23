using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneController))]
public class GameController : MonoBehaviour {

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

    private PlayerController _playerController;
    private SceneController _sceneController;

    void Awake()
    {
        _playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _sceneController = GetComponent<SceneController>();
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
            _sceneController.PressButton();
            panelPause.gameObject.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (!panelGameOver.gameObject.activeSelf)
        {
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
        _sceneController.PressButton();
        panelPause.gameObject.SetActive(false);
    }

}