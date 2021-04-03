using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Conversation Data
 * Data class. Stores information from all chatnodes, built by NodeController. 
 */
public class ConversationData {

    List<Dictionary<string, string>> allOrderedNodeData;

    public ConversationData() {
        allOrderedNodeData = new List<Dictionary<string, string>>();
    }

    public void AddNodeEntry(Dictionary<string, string> nodeData) {
        allOrderedNodeData.Add(nodeData);
    }

    public void ExportDataASJSON() {
        throw new NotImplementedException();
    }

    public string PrintData() {

        string ret = "";

        foreach(Dictionary<string, string> nodeEntry in allOrderedNodeData) {
            foreach(KeyValuePair<string, string> entry in nodeEntry) {
                ret += (entry.Key + " : " + entry.Value + "\n");
            }
            ret += ("\n");
        }

        return ret;
    }
}
