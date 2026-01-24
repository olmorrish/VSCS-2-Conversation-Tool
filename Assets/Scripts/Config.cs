using System.IO;
using SimpleJSON;
using UnityEngine;

/// <summary>
/// Contains global references to game meta state (version number, game content path, etc.)
/// Persists between scenes and pulls in data from config.json on game load.
/// Readonly and immutable after game launch. 
/// </summary>
public class Config : MonoBehaviour  {

    public static Config instance { get; private set; }

    private const string CONFIG_FILE_PATH = "config.json"; // in StreamingAssets
    
    // Data from config.json
    private string version; // version of the convo tool, ex: 1.0.0
    private string convoFolderFilePath; // folder to read/write convo file to, in the actual game project
    
    
    public string GetVersion() {
        return version;
    }
    
    public string GetConvoFolderFilePath() {
        return convoFolderFilePath;
    }

    private void LoadConfigData() {

        string filePath = Path.Combine(Application.streamingAssetsPath, CONFIG_FILE_PATH);
        if (File.Exists(filePath)) {
            // Load the file contents and parse as a JSONNode
            string configFileContents = File.ReadAllText(filePath);
            TextAsset textAsset = new TextAsset(configFileContents);
            JSONNode configJSON = JSON.Parse(textAsset.text);

            JSONNode buildConfigSection = configJSON["build"];
            version = buildConfigSection["version"].Value;
            convoFolderFilePath = buildConfigSection["convoFolderFilePath"].Value;

            Debug.Log("Config data was loaded successfully for version " + version + ".");
        }
        else {
            Debug.LogError("Could not file config file at path: \"" + filePath + "\"!");
        }
    }
    
    private void Awake() {
        // Config is a singleton that persists between scenes. This is only called ONCE.
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
        else {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfigData();
        }
    }
}