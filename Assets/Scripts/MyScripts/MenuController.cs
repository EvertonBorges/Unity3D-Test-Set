using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [SerializeField]
    private Image panelSettings;

    [SerializeField]
    private Toggle toggleAi;

    [SerializeField]
    private Toggle toggleCoins;

    [SerializeField]
    private Text labelCoins;

    [SerializeField]
    private SceneController sceneController;

    void Awake()
    {
        panelSettings.gameObject.SetActive(false);
    }

    public void ShowSettings()
    {
        Settings settings = ConfigurationsSingleton.Instance.settings;
        toggleAi.isOn = settings.aiEnable;
        toggleCoins.isOn = settings.aiCoinsEnable;

        panelSettings.gameObject.SetActive(true);
        sceneController.PressButton();
    }

    public void HideSettings()
    {
        panelSettings.gameObject.SetActive(false);
        sceneController.PressButton();
    }

    public void Save()
    {
        Settings settings = new Settings();
        settings.aiEnable = toggleAi.isOn;
        settings.aiCoinsEnable = toggleCoins.isOn;

        ConfigurationsSingleton.Instance.Save(settings);
        panelSettings.gameObject.SetActive(false);

        sceneController.PressButton();
    }

    public void SetAI()
    {
        if (toggleAi.isOn)
        {
            Color color = labelCoins.color;
            color.a = 1f;
            labelCoins.color = color;
            toggleCoins.interactable = true;
        }
        else
        {
            Color color = labelCoins.color;
            color.a = 0.5f;
            labelCoins.color = color;
            toggleCoins.interactable = false;
            toggleCoins.isOn = false;
        }
    }

}