using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportButton : MonoBehaviour {

    public TMPro.TextMeshProUGUI buttonTextmesh; 
    private NodeController nodeController;
    private ImportPopupWindow importWindow;
    private string fileFqn;

    /// <summary>
    /// Initializes the button. Called by ImportPopupWindow when it spawns the button.
    /// </summary>
    /// <param name="fileName"></param>
    public void Init(string fileFqn, string displayName, ImportPopupWindow importWindow) {
        this.importWindow = importWindow;
        nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
        buttonTextmesh.text = displayName;
        this.fileFqn = fileFqn;
    }

    /// <summary>
    /// Called by button that this is attached to.
    /// </summary>
    public void Import() {
        nodeController.Import(fileFqn);
        importWindow.CloseWindow();
    }
    
}
