﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariantPanelAddFeedArticle : VariantPanel {

    public TMPro.TMP_InputField articleIDInputField;
    public ConnectionNub nextNub;

    public override Dictionary<string, string> GetVariantPanelData() {
        Dictionary<string, string> ret = new Dictionary<string, string>();
        ret.Add("param", articleIDInputField.text);

        ConnectionNub nubOnNextNode = nextNub.connectedNub;
        ret.Add("next", nubOnNextNode == null ? "TERMINATE" : nubOnNextNode.GetParentChatNode().GetID());

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {
        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "param":
                    articleIDInputField.text = pair.Value.ToString();
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
}
