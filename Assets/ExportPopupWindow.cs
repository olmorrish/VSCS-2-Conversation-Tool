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
        if (nodeController.currentFileName != string.Empty)
            detectedFile.text = "The current file is: \"" + nodeController.currentFileName + "\"";
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
            if (customExportNameField.text == "" || customExportNameField.text == nodeController.currentFileName) {
                statusText.color = green;
                statusText.text = "This file will be exported with previous name as: \"" + nodeController.currentFileName + "\".json";
                exportOkay = true;
            }

            //otherwise, there is a new filename that doesn't match, so show a warning
            else {
                statusText.color = yellow;
                statusText.text = "This file was imported as: \"" + nodeController.currentFileName + ".json\", which differs from the given export name: \"" + customExportNameField.text + "\".json. Please ensure this is correct, or leave the Custom Export Name blank to export using the previous import name.";
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
                statusText.text = "This new file will be exported as: \"" + customExportNameField.text + "\".json";
                exportOkay = true;
            }
        }




    }

    public void CloseWindow() {
        Destroy(gameObject);
    }

    public void ExportFile() {
        if (exportOkay) {
            nodeController.Export(customExportNameField.text == "" ? nodeController.currentFileName : customExportNameField.text);
            CloseWindow();
        }
    }
}
