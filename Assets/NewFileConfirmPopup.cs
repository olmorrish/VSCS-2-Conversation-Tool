using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFileConfirmPopup : MonoBehaviour {

    NodeController nodeController;

    // Start is called before the first frame update
    void Awake() {
    nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
    }

    public void NewFileConfirmed() {
        nodeController.outputText.AddLine("New file created, screen has been cleared.");
        nodeController.NewFile();
        Destroy(gameObject);
    }

    public void CloseWindow() {
        Destroy(gameObject);
    }
}
