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
        throw new System.NotImplementedException();
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
}
