using UnityEngine;

[CreateAssetMenu(fileName = "AudioCatalog", menuName = "Audio/Audio Catalog")]
public class AudioCatalog : ScriptableObject {
	[Header("=== BGM ===")]
	public AudioClip battleBgm;

	[Header("=== SFX ===")]
	public AudioClip cardUse;
	public AudioClip hit;
	public AudioClip relicTrigger;
	public AudioClip turnEnd;
	public AudioClip victory;
	public AudioClip defeat;

	public AudioClip GetSfx(GameAudioCue cue) {
		return cue switch {
			GameAudioCue.CardUse => cardUse,
			GameAudioCue.Hit => hit,
			GameAudioCue.RelicTrigger => relicTrigger,
			GameAudioCue.TurnEnd => turnEnd,
			GameAudioCue.Victory => victory,
			GameAudioCue.Defeat => defeat,
			_ => null
		};
	}
}
