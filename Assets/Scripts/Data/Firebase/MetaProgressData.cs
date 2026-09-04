using System;

[Serializable]
public class MetaProgressData {
	public int gold;
	public string[] unlockedRelicIds;
	public string[] purchasedRelicIds;
	public long updatedAtUnixTime;
}
