using UnityEngine;

public static class BuildLogSilencer {
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void DisablePlayerLogs() {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
		Debug.unityLogger.logEnabled = false;
#endif
	}
}
