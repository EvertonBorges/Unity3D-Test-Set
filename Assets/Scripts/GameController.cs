using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [SerializeField]
    private Image panelGameOver;

    [SerializeField]
    private Text textCoins;

    [SerializeField]
    private Text textScore;

    [SerializeField]
    private Text textDistance;

    private bool isPause = false;

    void Start() {
        panelGameOver.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (isPause) return;
    }

    public void Pause() {
        isPause = true;
        panelGameOver.gameObject.SetActive(true);
    }

    public void UpdateCoins(string coins)
    {
        textCoins.text = coins;
    }

    public void UpdateScore(string score)
    {
        textScore.text = score;
    }

    public void UpdateDistance(string distance)
    {
        textDistance.text = distance + " m";
    }

}