using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariantPanelNote : VariantPanel {

    public TMPro.TMP_InputField noteTextInputField;
    public Toggle toggle;
    public Image bodyPanelImage;

    // Update is called once per frame
    void Update() {
        bodyPanelImage.color = toggle.isOn ? Color.yellow : Color.white;
    }

    public override Dictionary<string, string> GetVariantPanelData() {

        Dictionary<string, string> ret = new Dictionary<string, string>();

        ret.Add("text", noteTextInputField.text);
        ret.Add("todobool", toggle.isOn ? "true" : "false");

        return ret;
    }

    public override void PopulateVariantPanelData(Dictionary<string, string> savedData) {

        foreach (KeyValuePair<string, string> pair in savedData) {
            switch (pair.Key) {
                case "text":
                    noteTextInputField.text = pair.Value.ToString();
                    break;
                case "todobool":
                    toggle.isOn = pair.Value.Equals("true");
                    break;
            }
        }
    }

    public override List<ChatNode> GetDescendantChatNodes() {
        return new List<ChatNode> { };
    }

    public override List<ConnectionNub> GetNubs() {
        return new List<ConnectionNub> { };
    }
}
