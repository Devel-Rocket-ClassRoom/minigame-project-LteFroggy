using UnityEngine;

public class DescriptionPanelPool : SingletonPoolBase<DescriptionPanelPool, DescriptionPanelController> {
	public DescriptionPanelController Get(string title, string description, Transform parent, Sprite icon = null) {
		var panel = base.Get();
		panel.transform.SetParent(parent, false);
		panel.SetContent(title, description, icon);

		return panel;
	}
}