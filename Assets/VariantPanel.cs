using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VariantPanel {

    public abstract Dictionary<string, string> GetNodeData();
    public abstract void PopulateNodeData(Dictionary<string, string> savedData);
}
