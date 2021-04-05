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

        GameObject[] allChatNodesObjects = GameObject.FindGameObjectsWithTag("Node");
        ChatNode[] allChatNodes = new ChatNode[allChatNodesObjects.Length];

        //iterate over chatnode objects to find the head and get all ChatNode components
        ChatNode headNode = null;
        for (int i = 0; i<allChatNodesObjects.Length; i++) {
            allChatNodes[i] = allChatNodesObjects[i].GetComponent<ChatNode>();
            if (allChatNodes[i].GetID().Equals(headNodeID)) {
                headNode = allChatNodes[i];
            }
        }

        if(headNode == null) {
            Debug.LogWarning("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            return;
        }

        //create a convodata then recursively go over each node and get it's data
        ConversationData allData = new ConversationData();
        allData.AddNodeEntry(headNode.GetChatNodeData());

        //topologically sort all nodes in the scene
        TopologicalSortNodes(allChatNodes, headNode);


        Debug.Log(allData.PrintData()); //TODO REMOVE DEBUG

        //TODO once data is collected, error check, ex: for duplicate node IDs

        //TODO once data is collected, convert to a JSON file and save it
    }

    #region Topological Sort Functions

    /* 
     * Utilizes DFS to topologically sort all chatnodes, such that all predecessors of a given node appear before it in the order.
     * This is required due to the way the VSCS-II chat system build the chattree from the JSON; they must be ordered topologically before saving them to JSON.
     * ALG REF: https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
     * Returns: Nothing; stores nodes in local variable sortedNodes. Cleaner signatures. 
     */
    private void TopologicalSortNodes(ChatNode[] nodesToSort, ChatNode headNode) {

        //set up the marks, all false by default 
        temporaryMarks = new Dictionary<string, bool>();
        permanentMarks = new Dictionary<string, bool>();
        foreach(ChatNode node in nodesToSort) {
            temporaryMarks[node.GetID()] = false;
            permanentMarks[node.GetID()] = false;
        }

        //DFS topological sort, loop until all a nodes permanently marked
        sortedNodes = new List<ChatNode>();

        VisitNode(headNode);

        //TODO: remove this debug print
        string s = "";
        foreach (ChatNode sortedNode in sortedNodes) {
            s += sortedNode.GetID() + " ,";
        }
        Debug.Log("TOP. ORDERED NODES: " + s);
    }


    /*
     * Support method. Resursively calls Visit() on direct descendants
     */
    private void VisitNode(ChatNode n) {

        if (permanentMarks[n.GetID()])
            return;
        if (temporaryMarks[n.GetID()]) {
            Debug.LogError("ERROR EXPORTING: A cycle in the graph was detected. Conversations must be directed acyclic graphs (DAGs).");
            return;
        }

        temporaryMarks[n.GetID()] = true;

        foreach(ChatNode m in n.GetDescendantNodes()) {
            VisitNode(m);
        }

        temporaryMarks[n.GetID()] = false;
        permanentMarks[n.GetID()] = true;

        sortedNodes.Insert(0, n);

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

    #endregion
}
