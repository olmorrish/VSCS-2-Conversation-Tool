using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelBranchOnStoredBool : VariantPanel {

    [Header("Internal References")]
    public TMPro.TMP_InputField boolNameInputField;
    public ConnectionNub nextNubTrue;
    public ConnectionNub nextNubFalse;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();

        ret.Add("param", boolNameInputField.text);

        ConnectionNub nubOnNextNode = nextNubTrue.connectedNub;
        ret.Add("nextT", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        nubOnNextNode = nextNubFalse.connectedNub;
        ret.Add("nextF", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "param":
                    boolNameInputField.text = pair.Value.ToString();
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        List<ChatNode> nexts = new List<ChatNode>();

        if (nextNubTrue.connectedNub != null) { //either one of the connections may be null
            nexts.Add(nextNubTrue.connectedNub.GetParentChatNode());
        }
        if (nextNubFalse.connectedNub != null) {
            nexts.Add(nextNubFalse.connectedNub.GetParentChatNode());
        }

        return nexts;
    }

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub>() { nextNubTrue, nextNubFalse };
    }
}
