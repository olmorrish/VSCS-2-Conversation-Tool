using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantPanelBranchOnPlayerInput : VariantPanel {

    public TMPro.TMP_InputField optionAInputField;
    public TMPro.TMP_InputField optionBInputField;
    public TMPro.TMP_InputField optionCInputField;

    public Toggle optionBToggle;
    public Toggle optionCToggle;

    public ConnectionNub nextANub;
    public ConnectionNub nextBNub;
    public ConnectionNub nextCNub;

    private void Update() {

        optionBInputField.enabled = optionBToggle.isOn;
        optionCInputField.enabled = optionCToggle.isOn;

        if (!optionBToggle.isOn) {
            nextBNub.DisconnectNub();
        }
        if (!optionCToggle.isOn) {
            nextCNub.DisconnectNub();
        }
    }

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();

        ret.Add("optionA", optionAInputField.text);
        ConnectionNub nubOnNextNode = nextANub.connectedNub;
        ret.Add("nextA", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        if (optionBToggle.isOn) {
            ret.Add("optionB", optionBInputField.text);
            nubOnNextNode = nextBNub.connectedNub;
            ret.Add("nextB", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());
        }

        if (optionCToggle.isOn) {
            ret.Add("optionC", optionCInputField.text);
            nubOnNextNode = nextCNub.connectedNub;
            ret.Add("nextC", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());
        }

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "optionA":
                    optionAInputField.text = pair.Value.ToString();
                    break;
                case "optionB":
                    optionBInputField.text = pair.Value.ToString();
                    break;
                case "optionC":
                    optionCInputField.text = pair.Value.ToString();
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        List<ChatNode> nexts = new List<ChatNode>();

        if (nextANub.connectedNub != null) { //either one of the connections may be null
            nexts.Add(nextANub.connectedNub.GetParentChatNode());
        }
        if (nextBNub.connectedNub != null && optionBToggle.isOn) {
            nexts.Add(nextBNub.connectedNub.GetParentChatNode());
        }
        if (nextCNub.connectedNub != null && optionCToggle.isOn) {
            nexts.Add(nextCNub.connectedNub.GetParentChatNode());
        }

        return nexts;
    }
}
