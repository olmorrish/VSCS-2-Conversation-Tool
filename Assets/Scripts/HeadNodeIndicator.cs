using UnityEngine;

public class HeadNodeIndicator : MonoBehaviour {
    
    private ConnectionNub outgoingNub;
    
    /// <summary>
    /// Gets outgoing nubs, which can then be connected tothe head node. Obtained when making connections on import.
    /// </summary>
    /// <returns>Outgoing nubs.</returns>
    public ConnectionNub GetOutgoingNub() {
        return outgoingNub;
    }

    /// <summary>
    /// Gets the descendant of this Indicator, which is designated as the head node and serialized first.
    /// </summary>
    /// <returns>A list of descendant ChatNodes.</returns>
    public ChatNode GetDescendantHeadNode() {
        return outgoingNub.connectedNub?.GetParentChatNode();
    }
}
