using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Settings
{
    // Enable/Disable AI controller
    public bool aiEnable;
    // Enable/Disable AI focus in get coins (fishes)
    public bool aiCoinsEnable;

    public Settings()
    {
        this.aiEnable = false;
        this.aiCoinsEnable = false;
    }
}

public class ConfigurationsSingleton : MonoBehaviour
{

    private static ConfigurationsSingleton _instance;

    private string _filePath;
    private Settings _settings;
    public Settings settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = new Settings();
            }
            return _settings;
        }
    }

    public static ConfigurationsSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("Configurations");
                ConfigurationsSingleton component = gameObject.AddComponent<ConfigurationsSingleton>();
                _instance = component;
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _filePath = Application.persistentDataPath + "/settings.dat";
            _instance = this;

            if (File.Exists(_filePath))
            {
                Load();
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Save settings in internal file
    public void Save(Settings datas)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(_filePath);

        binaryFormatter.Serialize(file, datas);
        file.Close();

        _settings = datas;
    }

    // Load the settings in internal file
    private void Load()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Open(_filePath, FileMode.Open);

        Settings datas = (Settings)binaryFormatter.Deserialize(file);
        file.Close();

        _settings = datas;
    }

}