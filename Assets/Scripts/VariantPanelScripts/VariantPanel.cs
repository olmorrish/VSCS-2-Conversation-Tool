﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VariantPanel : MonoBehaviour {

    public abstract Dictionary<string, string> GetVariantPanelData();
    public abstract void PopulateVariantPanelData(Dictionary<string, string> savedData);
    public abstract List<ChatNode> GetDescendantChatNodes();
    public abstract List<ConnectionNub> GetNubs();

}
