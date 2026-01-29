using System.Collections.Generic;
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

        ret.Add(Constants.KEY_OPTION_A, optionAInputField.text);
        ConnectionNub nubOnNextNode = nextANub.connectedNub;
        ret.Add(Constants.KEY_NEXT_NODE_A, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());

        if (optionBToggle.isOn) {
            ret.Add(Constants.KEY_OPTION_B, optionBInputField.text);
            nubOnNextNode = nextBNub.connectedNub;
            ret.Add(Constants.KEY_NEXT_NODE_B, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());
        }

        if (optionCToggle.isOn) {
            ret.Add(Constants.KEY_OPTION_C, optionCInputField.text);
            nubOnNextNode = nextCNub.connectedNub;
            ret.Add(Constants.KEY_NEXT_NODE_C, nubOnNextNode == null ? Constants.VALUE_TERMINATE : nubOnNextNode.GetParentChatNode().GetID());
        }

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        bool optionBWasInData = false; // B and C might not even be in the saved data
        bool optionCWasInData = false;

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case Constants.KEY_OPTION_A:
                    optionAInputField.text = pair.Value;
                    break;
                case Constants.KEY_OPTION_B:
                    optionBInputField.text = pair.Value;
                    optionBWasInData = true;
                    break;
                case Constants.KEY_OPTION_C:
                    optionCInputField.text = pair.Value;
                    optionCWasInData = true;
                    break;
            }
        }

        // activate B and C toggles if we just populated those; else deactivate
        optionBToggle.isOn = optionBWasInData;
        optionCToggle.isOn = optionCWasInData;
    }

    public override List<ChatNode> GetDescendantChatNodes() {

        List<ChatNode> nexts = new List<ChatNode>();

        if (nextANub.connectedNub != null) { // some of the connections may be null
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

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub>() { nextANub, nextBNub, nextCNub };
    }
}
