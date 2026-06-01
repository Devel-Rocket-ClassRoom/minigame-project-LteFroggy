using UnityEngine;

public class DescriptionPanelPool : SingletonPoolBase<DescriptionPanelPool, DescriptionPanelController> {
	public DescriptionPanelController Get(string title, string description, Transform parent, Sprite icon = null) {
		var panel = base.Get();
		panel.SetContent(title, description, icon);
		panel.transform.SetParent(parent, false);

		return panel;
	}
}