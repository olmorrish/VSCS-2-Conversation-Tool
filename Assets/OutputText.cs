using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutputText : MonoBehaviour {

    public TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start() {
        textMesh.text = ("Info and errors display here.");
    }

    public void AddLine(string line) {
        textMesh.text = line + "\n" + textMesh.text;
    }
}
