using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportPopupWindow : MonoBehaviour {

    private static ExportPopupWindow instance; //exportwindow is a singleton, but new instances take precendent
    private NodeController nodeController;
    public TMPro.TextMeshProUGUI detectedFile;
    public TMPro.TMP_InputField customExportNameField;

    public TMPro.TextMeshProUGUI statusText;
    public Color green;
    public Color yellow;
    public Color red;

    private bool exportOkay;

    // Start is called before the first frame update
    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(instance.gameObject);
            instance = this;
        }

        nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
        if (nodeController.currentFileFQN != string.Empty)
            detectedFile.text = "The current file is: \"" + nodeController.currentFileFQN + "\"";
        else {
            detectedFile.text = "There is no file currently loaded.";
        }

        exportOkay = false;
    }

    // Update is called once per frame
    void Update() {

        //if a file is currently loaded...
        if (nodeController.fileLoaded) {

            //if the field is blank or matches, export with the current name
            if (customExportNameField.text == "") {
                statusText.color = green;
                statusText.text = "This file will be exported to its previous file path as: \"" + nodeController.currentFileFQN;
                exportOkay = true;
            }

            //otherwise we're exporting an existing file under a new name
            else {
                statusText.color = yellow;
                statusText.text = "This file was imported as: \"" + nodeController.currentFileFQN + "\". Make sure you want to export to the Resources folder as: \"" + customExportNameField.text + ".json\". Leave the Custom Export Name blank to export using the previous import name and path.";
                exportOkay = true;
            }
        }

        //else no file is loaded
        else {
            //if there's no export name, error! Otherwise, ok.
            if (customExportNameField.text == "") {
                statusText.color = red;
                statusText.text = "This is a new file. To export, give a new file name.";
                exportOkay = false;
            }
            else {
                statusText.color = green;
                statusText.text = "This new file will be exported to the Resources folder as: \"" + customExportNameField.text + ".json\"";
                exportOkay = true;
            }
        }
    }

    public void CloseWindow() {
        Destroy(gameObject);
    }

    public void ExportFile() {
        if (exportOkay) {

            string customName = customExportNameField.text; 

            if (customName == "") {
                nodeController.Export();
            }
            else {
                nodeController.ExportNewFile(customName);
            }
            
            CloseWindow();
        }
    }
}
