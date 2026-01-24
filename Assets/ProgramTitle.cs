using TMPro;
using UnityEngine;

public class ProgramTitle : MonoBehaviour {

    private const string BASE_TITLE = "VSCS Convo Tool v";
    private TextMeshProUGUI titleTextMesh;
    
    void Start() {
        titleTextMesh = GetComponent<TextMeshProUGUI>();
        string version = Config.instance.GetVersion();
        titleTextMesh.text = BASE_TITLE + version;
    }
}
