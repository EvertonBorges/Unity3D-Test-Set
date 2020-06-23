using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SceneController : MonoBehaviour {

    [SerializeField]
    private AudioClip _pressButtonSfx;

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Exit() {
        PressButton();
        Application.Quit();
    }

    public void Play() {
        PressButton();
        SceneManager.LoadScene("Game");
    }

    public void Menu() {
        PressButton();
        SceneManager.LoadScene("Menu");
    }

    public void PressButton()
    {
        _audioSource.clip = _pressButtonSfx;
        _audioSource.Play();
    }

}