using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System;
using System.IO;

public class NodeController : MonoBehaviour {

    [Header("References")]
    public GameObject ChatNodePrefab;
    public TMPro.TMP_InputField headIDInputField;
    public TMPro.TMP_InputField exportNameInputField;
    public OutputText outputText;

    //[Header("Data")]
    private string headNodeID;

    //support data for topological sort; reset each time
    private Dictionary<string, bool> temporaryMarks;
    private Dictionary<string, bool> permanentMarks;
    private List<ChatNode> sortedNodes;

    #region Button Fuctions

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

        //disconnect all nubs on the copy (one way); otherwise the connections persist
        ConnectionNub[] allConnectionNubs = newChatNode.GetComponentsInChildren<ConnectionNub>();
        foreach (ConnectionNub nub in allConnectionNubs)
            nub.DisconnectThisNubOnly(); //leaves other end intact, only affects new nub
    }

    /*
     * Starting at the marked head node, called by export button.
     */
    public void Export() {

        //find the head node
        string headNodeID = headIDInputField.text;
        string exportName = exportNameInputField.text;

        if (headNodeID.Equals("")) {
            outputText.AddLine("ERROR EXPORTING: No head node ID was specified.");
            Debug.LogWarning("ERROR EXPORTING: No head node ID was specified.");
            return;
        }
        else if (exportName.Equals("")) {
            outputText.AddLine("ERROR EXPORTING: No export name was specified.");
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
            outputText.AddLine("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            Debug.LogWarning("ERROR EXPORTING: Head node with ID \"" + headNodeID + "\" could not be found.");
            return;
        }

        //create a convodata then recursively go over each node and get it's data
        ConversationData allData = new ConversationData();
        allData.AddNodeEntry(headNode.GetChatNodeData());

        //topologically sort all nodes in the scene; this stores them in sortedNodes
        TopologicalSortNodes(allChatNodes, headNode);

        Debug.Log(allData.PrintData()); //TODO REMOVE DEBUG

        //error check sorted nodes for duplicate IDs
        List<string> allIDs = new List<string>();
        foreach (ChatNode node in sortedNodes) {
            string id = node.GetID();
            if (allIDs.Contains(id)) {
                outputText.AddLine("The ID \"" + id + "\" appears more than once in the exported nodes. This will likely cause an issue upon ChatSystem interpretation.");
                Debug.LogWarning("The ID \"" + id + "\" appears more than once in the exported nodes. This will likely cause an issue upon ChatSystem interpretation.");
            }
            else {
                allIDs.Add(id);
            }
        }


        //get a dictionary of entries for each node
        List<Dictionary<string, string>> nodeEntries = new List<Dictionary<string, string>>();
        foreach(ChatNode node in sortedNodes) {
            nodeEntries.Add(node.GetChatNodeData());
        }

        //convert dictionaries to a JSON array
        JSONArray allNodes = new JSONArray();

        foreach(Dictionary<string, string> nodeEntry in nodeEntries) {

            //turn each ChatNode into a JSON obj...
            JSONObject singleNode = new JSONObject();
            foreach (KeyValuePair<string, string> pair in nodeEntry) {
                singleNode.Add(pair.Key, pair.Value);
            }

            //... then add it to the array
            allNodes.Add(singleNode);
        }

        Debug.Log(allNodes.ToString());

        //write the JSON
        string saveFilePath = Application.persistentDataPath + "\\" +  exportNameInputField.text + ".json";
        File.WriteAllText(saveFilePath, allNodes.ToString());

        outputText.AddLine("SUCCESS! Exported file as \"" + exportNameInputField.text + ".json\" (in AppData).");
    }

    public void Import() {

        if (exportNameInputField.text.Equals("")) {
            outputText.AddLine("ERROR EXPORTING: No import name was specified.");
            Debug.LogWarning("ERROR EXPORTING: No import name was specified.");
            return;
        }

        string saveFilePath = Application.persistentDataPath + "\\" +  exportNameInputField.text + ".json";

        throw new NotImplementedException();
    }

    public void QuitApplication() {
        Application.Quit();
    }

    #endregion

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
        //string s = "";
        //foreach (ChatNode sortedNode in sortedNodes) {
        //    s += sortedNode.GetID() + " ,";
        //}
        //Debug.Log("TOP. ORDERED NODES: " + s);
    }


    /*
     * Support method. Resursively calls Visit() on direct descendants
     */
    private void VisitNode(ChatNode n) {

        if (permanentMarks[n.GetID()])
            return;
        if (temporaryMarks[n.GetID()]) {
            outputText.AddLine("ERROR EXPORTING: A cycle in the graph was detected. Conversations must be directed acyclic graphs (DAGs).");
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
