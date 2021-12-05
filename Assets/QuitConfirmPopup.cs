using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitConfirmPopup : MonoBehaviour {

    NodeController nodeController;

    // Start is called before the first frame update
    void Awake() {
        nodeController = GameObject.Find("NodeController").GetComponent<NodeController>();
    }

    public void QuitConfirmed() {
        nodeController.QuitApplication();
    }

    public void CloseWindow() {
        Destroy(gameObject);
    }
}
