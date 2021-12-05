using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportButton : MonoBehaviour {

    public TMPro.TextMeshProUGUI buttonTextmesh; 
    private NodeController nodeController;
    private ImportPopupWindow importWindow;
    private string fileName;

    /// <summary>
    /// Initializes the button. Called by ImportPopupWindow when it spawns the button.
    /// </summary>
    /// <param name="fileName"></param>
    public void Init(string fileName, ImportPopupWindow importWindow) {
        this.importWindow = importWindow;
        nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
        buttonTextmesh.text = fileName;
        this.fileName = fileName;
    }

    /// <summary>
    /// Called by button that this is attached to.
    /// </summary>
    public void Import() {
        nodeController.Import(fileName);
        importWindow.CloseWindow();
    }
    
}
