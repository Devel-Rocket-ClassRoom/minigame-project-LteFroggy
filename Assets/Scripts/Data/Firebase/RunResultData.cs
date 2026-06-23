[System.Serializable]
public class RunResultData {
	public string result;
	public string enemyEncounterId;
	public int[] enemyIds;
	public int currentHealth;
	public int maxHealth;
	public int gold;
	public int deckCount;
	public int relicCount;
	public string[] cardIds;
	public string[] relicIds;
	public long savedAtUnixTime;
}
