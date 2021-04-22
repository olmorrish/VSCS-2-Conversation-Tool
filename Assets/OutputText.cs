using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OutputText : MonoBehaviour {

    public TextMeshProUGUI textMesh;
    public float fadeSpeed;

    // Start is called before the first frame update
    void Start() {
        textMesh.text = ("Info and errors display here.");
    }

    private void Update() {

        float r = Mathf.Lerp(textMesh.color.r, Color.white.r, fadeSpeed);
        float g = Mathf.Lerp(textMesh.color.g, Color.white.g, fadeSpeed);
        float b = Mathf.Lerp(textMesh.color.b, Color.white.b, fadeSpeed);

        textMesh.color = new Color(r, g, b);
    }


    public void AddLine(string line) {
        textMesh.text = line + "\n" + textMesh.text;

        if (line.Contains("ERROR"))
            textMesh.color = Color.red;
        else if (line.Contains("SUCCESS"))
            textMesh.color = Color.green;
        else
            textMesh.color = Color.yellow;

    }
}
