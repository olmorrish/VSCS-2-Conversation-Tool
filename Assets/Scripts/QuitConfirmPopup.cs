using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitConfirmPopup : MonoBehaviour {

    private NodeController nodeController;
    private static QuitConfirmPopup instance; //this popup is a singleton, but new instances take precendent

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

    public void QuitConfirmed() {
        nodeController.QuitApplication();
    }

    public void CloseWindow() {
        Destroy(gameObject);
    }
}
