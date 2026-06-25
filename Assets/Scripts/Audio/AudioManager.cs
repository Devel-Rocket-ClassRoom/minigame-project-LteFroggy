using UnityEngine;

public enum GameAudioCue {
	CardUse,
	Hit,
	RelicTrigger,
	TurnEnd,
	Victory,
	Defeat
}

public class AudioManager : MonoBehaviour {
	private const string CatalogResourcesPath = "Audio/AudioCatalog";

	private static AudioManager _instance;
	public static AudioManager Instance {
		get {
			if (_instance != null) return _instance;
			_instance = FindAnyObjectByType<AudioManager>();
			if (_instance == null)
				_instance = new GameObject(nameof(AudioManager)).AddComponent<AudioManager>();
			return _instance;
		}
		private set => _instance = value;
	}

	[SerializeField, Range(0f, 1f)] private float _masterVolume = 1f;
	[SerializeField, Range(0f, 1f)] private float _sfxVolume = 0.75f;
	[SerializeField, Range(0f, 1f)] private float _bgmVolume = 0.35f;
	[SerializeField] private AudioCatalog _catalog;

	private AudioSource _sfxSource;
	private AudioSource _bgmSource;

	public float MasterVolume => _masterVolume;
	public float SfxVolume => _sfxVolume;
	public float BgmVolume => _bgmVolume;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap() {
		_ = Instance;
	}

	private void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(gameObject);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
		EnsureSources();
		LoadCatalog();
		ApplyVolumes();
	}

	public void PlaySfx(GameAudioCue cue, float volumeScale = 1f) {
		EnsureReady();
		AudioClip clip = _catalog != null ? _catalog.GetSfx(cue) : null;
		if (clip == null) return;

		_sfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
	}

	public void PlayBattleBgm() {
		EnsureReady();
		if (_catalog == null || _catalog.battleBgm == null) return;

		if (_bgmSource.clip == _catalog.battleBgm && _bgmSource.isPlaying) return;
		_bgmSource.clip = _catalog.battleBgm;
		_bgmSource.loop = true;
		_bgmSource.Play();
	}

	public void StopBgm() {
		if (_bgmSource == null) return;
		_bgmSource.Stop();
		_bgmSource.clip = null;
	}

	public void SetMasterVolume(float volume) {
		_masterVolume = Mathf.Clamp01(volume);
		ApplyVolumes();
	}

	public void SetSfxVolume(float volume) {
		_sfxVolume = Mathf.Clamp01(volume);
		ApplyVolumes();
	}

	public void SetBgmVolume(float volume) {
		_bgmVolume = Mathf.Clamp01(volume);
		ApplyVolumes();
	}

	private void EnsureReady() {
		EnsureSources();
		LoadCatalog();
		ApplyVolumes();
	}

	private void EnsureSources() {
		if (_sfxSource == null) {
			_sfxSource = gameObject.AddComponent<AudioSource>();
			_sfxSource.playOnAwake = false;
			_sfxSource.loop = false;
			_sfxSource.spatialBlend = 0f;
		}

		if (_bgmSource == null) {
			_bgmSource = gameObject.AddComponent<AudioSource>();
			_bgmSource.playOnAwake = false;
			_bgmSource.loop = true;
			_bgmSource.spatialBlend = 0f;
		}
	}

	private void LoadCatalog() {
		if (_catalog != null) return;
		_catalog = Resources.Load<AudioCatalog>(CatalogResourcesPath);
		if (_catalog == null)
			Debug.LogWarning($"[AudioManager] AudioCatalog를 찾을 수 없습니다: Resources/{CatalogResourcesPath}");
	}

	private void ApplyVolumes() {
		if (_sfxSource != null)
			_sfxSource.volume = _masterVolume * _sfxVolume;
		if (_bgmSource != null)
			_bgmSource.volume = _masterVolume * _bgmVolume;
	}
}
