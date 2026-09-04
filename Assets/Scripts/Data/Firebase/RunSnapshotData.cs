using System;

[Serializable]
public class RunSnapshotData {
	public int currentHealth;
	public int maxHealth;
	public int gold;
	public int totalGoldEarned;
	public int totalGoldSpent;
	public int deckCount;
	public int relicCount;
	public string[] cardIds;
	public string[] relicIds;
	public string currentNodeType;
	public int currentNodeLayer;
	public int currentNodeIndex;
	public string[] visitedNodeTypes;
	public long savedAtUnixTime;
}
