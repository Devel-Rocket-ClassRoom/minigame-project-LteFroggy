using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusRenderer : MonoBehaviour {
	[SerializeField] private Image StatusImage;
	[SerializeField] private TextMeshProUGUI StatusText;
	public StatusBase Status { get; set; }
	
	public void Init(StatusBase status) {
		Status = status;
		UpdateStatus();
	}
	
	public void UpdateStatus() {
		SetImage(Status.Icon);
		SetText(Status.TextToShow);
		Debug.Log($"{Status.GetType().Name} 턴 : {Status.TextToShow}");
	}
	
	public void SetImage(Sprite sprite) {
		StatusImage.sprite = sprite;
	}
	
	public void SetText(string text) {
		StatusText.text = text;
	}
}