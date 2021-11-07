using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelAddBCPApp : VariantPanel {

    [Header("Internal References")]
    public TMPro.TMP_InputField appNameInputField;
    public TMPro.TMP_InputField displayNameInputField;
    public TMPro.TMP_InputField bnetIDInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("param", appNameInputField.text);
        ret.Add("displayname", displayNameInputField.text);
        ret.Add("bnetid", bnetIDInputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "param":
                    appNameInputField.text = pair.Value.ToString();
                    break;
                case "displayname":
                    displayNameInputField.text = pair.Value.ToString();
                    break;
                case "bnetid":
                    bnetIDInputField.text = pair.Value.ToString();
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        if (nextNub.connectedNub == null) {
            return new List<ChatNode> { }; //no connection => no descendants
        }
        else {
            return new List<ChatNode> { nextNub.connectedNub.GetParentChatNode() };
        }
    }

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub>() { nextNub };
    }
}
