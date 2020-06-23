﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    public void Exit() {
        Application.Quit();
    }

    public void Play() {
        SceneManager.LoadScene("Game");
    }

    public void Menu() {
        SceneManager.LoadScene("Menu");
    }

}