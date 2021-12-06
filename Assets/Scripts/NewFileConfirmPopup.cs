using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFileConfirmPopup : MonoBehaviour {

    private NodeController nodeController;
    private static NewFileConfirmPopup instance; //this popup is a singleton, but new instances take precendent

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
