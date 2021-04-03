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

    /*
     * Starting at the marked head node, 
     */
    public void RetrieveAndSaveAllNodeData() {

        //find the head node
        string headNodeID = headIDInputField.text;

        if (headNodeID.Equals("")) {
            Debug.LogWarning("ERROR EXPORTING: No head node ID was specified.");
            return;
        }


        GameObject[] allNodes = GameObject.FindGameObjectsWithTag("Node");  
        ChatNode headNode = null;
        foreach(GameObject chatNodeObj in allNodes) {
            ChatNode chatNode = chatNodeObj.GetComponent<ChatNode>();
            if (chatNode.GetID().Equals(headNodeID)) {
                headNode = chatNode;
                break;
            }
        }

        if(headNode == null) {
            Debug.LogWarning("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            return;
        }

        //create a convodata then recursively go over each node and get it's data
        ConversationData allData = new ConversationData();
        allData.AddNodeEntry(headNode.GetChatNodeData());

        Debug.Log(allData.PrintData()); //TODO REMOVE DEBUG

    }
}
