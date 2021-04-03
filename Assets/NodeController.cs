using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour {

    [Header("References")]
    public GameObject ChatNodePrefab;
    public TMPro.TMP_InputField headIDInputField;
    public TMPro.TMP_InputField exportNameInputField;

    //[Header("Data")]
    private string headNodeID;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    /*
     */
    public void SpawnNewChatNode() {
        GameObject newChatNode = Instantiate(ChatNodePrefab, this.transform);
        newChatNode.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
    }

    /* Copy spawner, called by chatnodes
     */
    public void SpawnNewChatNode(GameObject toCopy) {
        GameObject newChatNode = Instantiate(toCopy, this.transform);
        newChatNode.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
    }
}
