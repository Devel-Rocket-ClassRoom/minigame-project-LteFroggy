using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserStatsPanelController : MonoBehaviour {
	[SerializeField] private Button _closeButton;
	[SerializeField] private Transform _resultListRoot;
	[SerializeField] private RunResultItemView _resultItemPrefab;
	[SerializeField] private TextMeshProUGUI _statusText;

	private void OnEnable() {
		if (_closeButton != null)
			_closeButton.onClick.AddListener(Hide);
	}

	private void OnDisable() {
		if (_closeButton != null)
			_closeButton.onClick.RemoveListener(Hide);
	}

	public void Show() {
		gameObject.SetActive(true);
		LoadStatsAsync().Forget();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void ShowPendingState() {
		ClearResults();
		SetStatus("통계 데이터를 불러오는 중입니다.");
	}

	public void SetStatus(string message) {
		if (_statusText != null)
			_statusText.text = message;
	}

	public void ClearResults() {
		if (_resultListRoot == null)
			return;

		foreach (Transform child in _resultListRoot)
			Destroy(child.gameObject);
	}

	private async UniTaskVoid LoadStatsAsync() {
		ShowPendingState();

		FirebaseBootstrapper bootstrapper = FirebaseBootstrapper.Instance;
		if (bootstrapper == null) {
			SetStatus("Firebase 초기화 정보를 찾을 수 없습니다.");
			return;
		}

		InitState state = await bootstrapper.WaitForInitializationAsync();
		if (!isActiveAndEnabled)
			return;

		if (state != InitState.Ready || bootstrapper.RunResultManager == null) {
			SetStatus("Firebase 통계 조회를 사용할 수 없습니다.");
			return;
		}

		var (success, error, results) = await bootstrapper.RunResultManager.LoadRunResults();
		if (!isActiveAndEnabled)
			return;

		if (!success) {
			SetStatus(string.IsNullOrEmpty(error) ? "통계 데이터를 불러오지 못했습니다." : error);
			return;
		}

		RenderResults(results);
	}

	private void RenderResults(List<RunResultData> results) {
		ClearResults();

		if (results == null || results.Count == 0) {
			SetStatus("저장된 런 결과가 없습니다.");
			return;
		}

		SetStatus($"저장된 런 결과 {results.Count}건");
		if (_resultListRoot == null || _resultItemPrefab == null)
			return;

		foreach (RunResultData result in results) {
			RunResultItemView item = Instantiate(_resultItemPrefab, _resultListRoot);
			item.SetData(result);
		}
	}
}
