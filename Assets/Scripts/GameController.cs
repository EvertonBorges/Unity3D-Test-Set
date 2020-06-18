using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    [SerializeField]
    private Image panelGameOver;

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

}