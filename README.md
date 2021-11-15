# VSCS-2-Conversation-Tool
A UI tool to create conversation flowcharts and export them in a format parseable by the VSCS-II chat system. 

<h2>About</h2>
This README isn't written a full tech specification of this tool, nor the VSCS-II Chat System. Although maintainability and documentation are still a priority, they are developed solely by myself. Instead, this document is meant to provide an "devlog-style" overview for anybody curious about how the program works, specifically regarding it's relationship to VSCS-II, the game which it was developed to help build.

<h2>Systems</h2>
In my game VSCS-II, a line of computer systems from 1986 are retrofitted with internet connectivity, and users communicate via a TOR-like network called "BNET". This is the (admittedly somewhat contrived) reason why the game has a chat system, which forms the core of the game. VSCS-II does not have any centralized logic or "game master", and instead has a set of core Systems (note the capital 'S'), which each handle different sections and mechanics in the game. For instance, the player can receive publicly-available articles through BNET and view them through their Feed, and all interaction for this mechanic is routed through the "FeedSystem". Systems act as intermediaries and handlers for their domain, and can also communicate with one another. This pattern ensures that scripts aren't always bumping into one other.

The System that interacts with others the most is also the heart of the game: the ChatSystem. The ChatSystem is the main driver for the chat interactions with NPCs in the game, and since most of the story and events are triggered based on the player's interaction with this System, it ends up affecting every other System in the game to some degree. In the aforementioned FeedSystem, articles don't just appear from nowhere; they have to be triggered at some specific point in the story, and the story is driven by the ChatSystem (at least moment-to-moment; there is a broader container System for game progress). The point being: nearly every change in the VSCS-II state and game "scenery" originates from the player interacting with the ChatSystem. 

The main takaway here? It means the System becomes fairly complex, and has to do a lot of heavy lifting. If that wasn't tough enough, the story in VSCS-II <em>isn't linear</em>, and it can branch and weave quite a bit, even within a single conversation. So: how does one handle conversations that need to follow multiple paths, which need to enact a ton of different actions with different parameters, and such that these conversations are still easily editable and debuggable?

The answer is of course simple data structures. By which I mean trees. 

<h2>ChatTrees and ChatNodes</h2>
Every conversation in VSCS-II, when running, is represented in memory by a "ChatTree", which is a collection of branching "ChatNodes". When a given conversation starts, the tree is loaded into the System as the current tree, and it starts by pointing at the head node. The location on the tree is saved, and the next node is activated when it's entered, and the pointer moves on based on the type of node it is. There are currently thirteen distinct ChatNode types. The simpler ones simply perform an action then move on with the conversation, like the AddFeedArticle node, which just sets an article to appear to the player by telling the FeedSystem what to do. Some nodes have different conditions before they continue, like the Dialogue node, which queues up some text, but only gives the go-ahead to move on once that text has been printed.

Some nodes are very complex because they can go multiple different ways. The BranchOnPlayerInput node is one of the most common, and can move the conversation down up to three paths based on the input a player chooses. So with all this variation, it became infeasible to represent a story through by hardcoding it; each line of dialogue, each choice, and each minute detail would require a ChatNode object to be created by hand. So the obvious choice was to abstract some of this - but how?

There are two levels of distance from the ChatTree-in-memory version of any given conversation, both of which move the process of building the game's story away from "programming" and closer to "actually writing".

<h2>JSON Intermediary Format</h2>
The first level of abstration uses the JSON file format, since it lends itself well to storing nodes of information with clearly defined labels. When VSCS-II wants to launch a conversation with a character, it knows which JSON file that conversation is stored in, so it first parses that JSON file to build a ChatTree. The JSON representation of a conversation stores a sequential ordering of nodes which can be added to an empty ChatTree one after another (this becomes important later). Each node has all the parameters to define it's ChatNode type (and they're error checked as they're parsed) as well as the nodes that are children of it (whether it be one node or many).

The JSON format is simple and worked well for basic conversations. But as the scope of the game grew, it became clear that it didn't make any sense to write the story in this format either. It works well for storing conversations, but I needed a way to write the story, then turn it into this intermediate JSON form. 

<h2>The Conversation Tool</h2>
This is where the VSCS-II Conversation Tool shows up. The conversation tool is a GUI application built in Unity that visualizes what will eventually be turned into a ChatTree in VSCS-II's memory. It also allows me, as the author of the game, to write the story in a visual medium and then convert it into gameplay (I'm saying this as though it's new - tools like Twine have been doing this for ages, but the novelty of creating my own custom system hasn't worn thin).

The UI of the Convo Tool looks like VSCS-II's windows, but colour-coded and less-pixelated. A dropdown can be used to swap the ChatNode type, which swaps out the center panel so the correct fields can be entered and eventually exported as parameters. The process of linking parent nodes to children is done by dragging lines to connect "nubs" on nodes, and multiple nodes can even be moved at once by clicking and dragging to select more than one at a time. JSON files can be loaded and saved directly from my working directory, and the layout of the tree is saved (though this takes a tiny bit of extra storage). 

A lot of these sound simple, and are features that we take for granted a lot of the time, and building them from (relative) scratch was a fun exercise.

<h2>Directed Acyclic Graphs</h2>
Though I've covered most of the basics about this tool (and left out the boring stuff), there's an interesting issue I ran into while building the Conversation Tool. I mentioned earlier that the process of building the ChatTree involves iterating over each JSON node, turing it into a ChatNode in memory, and adding it to the tree in sequence. The unintended side effect here is that in the JSON format, the child of any node <em>must</em> not precede it in the listing, otherwise it cannot be added below another node in the tree. There are a few workarounds if I wanted to change this, but I chose to keep it. As a result, I had to make sure that the Convo Tool would export to JSON while following this rule. 

For anyone who knows graph theory, you'll understand that my conversations are represented by directed acyclic graphs (DAGs). For anyone else: this essentially means that the way my nodes are connected and navigated follow a couple of rules. First, the graph/tree is navigated in only one direction; we don't return to any given node. In other words, it's <em>directed</em>. Second, the graph/tree doesn't loop back on itself, and conversations do not reach the same state twice (as opposed to games where you can exhaust dialogue options, like Elder Scrolls or Deus Ex; I intentionally didn't design the game in this way). In other words, it's also <em>acyclic</em>. Hence, a DAG: a directed acyclic graph. 

Luckily, there exists an algorithm for sorting nodes in a DAG such that a parent of any given node always preceeds it in a list. This algorithm is called a "topological sort" (or "topological ordering"). I specifically implemented a DFS-based approach, the details of which I'll leave out. The Wikipedia page with some pseudocode is linked below. 

<h2>Links/Further Reading</h2>
<ul>
<li>VSCS-II: https://store.steampowered.com/app/1528590/VSCSII/</li>
<li>My itch.io portfolio: https://olmorrish.itch.io/</li>
<li>Topological Sort: https://en.wikipedia.org/wiki/Topological_sorting</li>
</ul>
