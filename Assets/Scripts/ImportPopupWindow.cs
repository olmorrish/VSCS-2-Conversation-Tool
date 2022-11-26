using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImportPopupWindow : MonoBehaviour {

    private NodeController nodeController;
    public GameObject importButtonPrefab;
    public Transform contentGrid;
    private static ImportPopupWindow instance; //importwindow is a singleton, but new instances take precendent
    public TMPro.TextMeshProUGUI detectedFile;
    public GameObject warningMessage;

    // Start is called before the first frame update
    void Awake() {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(instance.gameObject);
            instance = this;
        }

        //find the nodecontroller and set the text for the current file
        nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
        if (nodeController.fileLoaded)
            detectedFile.text = "The current file is: \"" + nodeController.currentFileFQN + "\"";
        else {
            detectedFile.text = "There is no file currently loaded.";
            warningMessage.SetActive(false);
        }

        string[] files = Directory.GetFiles(nodeController.getResourcesPath(), "*.json", SearchOption.AllDirectories);
        foreach (string fileFqn in files) {
            GameObject newButton = Instantiate(importButtonPrefab, contentGrid);
            string[] fileNameSeparated = fileFqn.Split('\\');
            string displayName = fileNameSeparated[fileNameSeparated.Length-1].Split('.')[0];
            newButton.GetComponent<ImportButton>().Init(fileFqn, displayName, this);
        }
    }

    public void CloseWindow() {
        Destroy(gameObject);
    }
}
