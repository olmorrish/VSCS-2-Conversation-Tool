using System.Collections.Generic;
using UnityEngine;

public class VariantPanelBranchOnStoredBool : VariantPanel {

    [Header("Internal References")]
    public TMPro.TMP_InputField boolNameInputField;
    public ConnectionNub nextNubTrue;
    public ConnectionNub nextNubFalse;

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();

        ret.Add(Constants.KEY_BOOL_KEY, boolNameInputField.text);

        ConnectionNub nubOnNextNode = nextNubTrue.connectedNub;
        ret.Add(Constants.KEY_NEXT_NODE_TRUE, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());

        nubOnNextNode = nextNubFalse.connectedNub;
        ret.Add(Constants.KEY_NEXT_NODE_FALSE, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case Constants.KEY_BOOL_KEY:
                    boolNameInputField.text = pair.Value;
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        List<ChatNode> nexts = new List<ChatNode>();

        if (nextNubTrue.connectedNub != null) { // either one of the connections may be null
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
