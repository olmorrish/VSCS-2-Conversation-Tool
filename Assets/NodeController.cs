using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;

public class NodeController : MonoBehaviour {

    [Header("References")]
    public GameObject ChatNodePrefab;
    public TMPro.TMP_InputField headIDInputField;
    public TMPro.TMP_InputField exportNameInputField;

    //[Header("Data")]
    private string headNodeID;

    //support data for topological sort; reset each time
    private Dictionary<string, bool> temporaryMarks;
    private Dictionary<string, bool> permanentMarks;
    private List<ChatNode> sortedNodes;

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

        //copy the nodetype of the previous node; this doesn't copy over cleanly otherwise
        int dropdownIndex = toCopy.GetComponent<ChatNode>().nodetypeDropdown.value;
        newChatNode.GetComponent<ChatNode>().nodetypeDropdown.value = dropdownIndex;
    }

    /*
     * Starting at the marked head node, 
     */
    public void RetrieveAndSaveAllNodeData() {

        //find the head node
        string headNodeID = headIDInputField.text;
        string exportName = exportNameInputField.text;

        if (headNodeID.Equals("")) {
            Debug.LogWarning("ERROR EXPORTING: No head node ID was specified.");
            return;
        }
        else if (exportName.Equals("")) {
            Debug.LogWarning("ERROR EXPORTING: No export name was specified.");
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

        //TODO do this is a way that ensures all predecesors are before a given node in the order

        Debug.Log(allData.PrintData()); //TODO REMOVE DEBUG

        //TODO once data is collected, convert to a JSON file

    }

    #region Topological Sort Functions

    /* 
     * Utilizes DFS to topologically sort all chatnodes, such that all predecessors of a given node appear before it in the order.
     * This is required due to the way the VSCS-II chat system build the chattree from the JSON; they must be ordered topologically before saving them to JSON.
     * ALG REF: https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
     */
    private ChatNode[] TopologicalSortNodes(ChatNode[] nodesToSort) {

        //set up the marks, all false by default 
        temporaryMarks = new Dictionary<string, bool>();
        permanentMarks = new Dictionary<string, bool>();
        foreach(ChatNode node in nodesToSort) {
            temporaryMarks[node.GetID()] = false;
            permanentMarks[node.GetID()] = false;
        }

        //DFS topological sort, loop until all a nodes permanently marked
        sortedNodes = new List<ChatNode>();
        while (!AllNodesMarkedPermanent(nodesToSort)) {
            foreach(ChatNode n in nodesToSort) {
                if (permanentMarks[n.GetID()] == false) {
                    VisitNode(n);
                }
            }
        }

        throw new NotImplementedException();
    }

    /*
     * Support method. Returns false if any node is not marked permanently, true if all are marked.
     */
    private bool AllNodesMarkedPermanent(ChatNode[] nodes) {
        foreach (ChatNode node in nodes) {
            if (permanentMarks[node.GetID()] == false)
                return false;
        }

        return true;
    }

    /*
     * Support method. Resursively calls Visit() on direct descendants
     */
    private void VisitNode(ChatNode n) {

        //if()

    }

    #endregion
}
