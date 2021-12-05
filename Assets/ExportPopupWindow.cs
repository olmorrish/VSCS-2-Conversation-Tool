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


        //if the field is blank but something is loaded, export with the current name -- export is okay!
        if (customExportNameField.text == "" && nodeController.currentFileName != "") {
            statusText.color = green;
            statusText.text = "This file will be exported with previous name: \"" + nodeController.currentFileName + "\"";
            exportOkay = true;
        }

        //if there is a new non-blank filename, that doesn't match the current one, show a warning
        else if (customExportNameField.text != "" && customExportNameField.text != nodeController.currentFileName) {
            statusText.color = yellow;
            statusText.text = "This file was imported as: \"" + nodeController.currentFileName + "\", which differs from the given export name: \"" + customExportNameField.text + "\". Please ensure this is correct, or leave the Custom Export Name blank to export using the previous import name.";
            exportOkay = true;
        }

        //if this isn't an existing file and there's no export name, error!
        else if (customExportNameField.text == "" && nodeController.currentFileName == "") {
            statusText.color = red;
            statusText.text = "This is a new file. To export, give a new file name.";
            exportOkay = false;
        }

    }

    public void CloseWindow() {
        Destroy(gameObject);
    }

    public void ExportFile() {
        if(exportOkay)
            nodeController.Export(customExportNameField.text == "" ? nodeController.currentFileName : customExportNameField.text);
    }
}
