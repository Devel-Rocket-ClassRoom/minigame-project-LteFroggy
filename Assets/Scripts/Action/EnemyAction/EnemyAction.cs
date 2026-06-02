using UnityEngine;

public abstract class EnemyAction : ActionBase<EnemyActionContext> {
	public abstract string IntentIconName { get; }
	// 적 의도 아이콘 주소
	public Sprite IntentIcon => Resources.Load<Sprite>($"Sprites/Intents/{IntentIconName}");
	public abstract string IntentDescriptionTitle { get; }
	public abstract string IntentDescriptionKey { get; }
	// 적 의도 아이콘 밑에 표시될 텍스트
	public abstract string GetIntentTextWithContext(EnemyActionContext context);

	public virtual string GetIntentDescriptionWithContext(EnemyActionContext context) {
		return StringTableManager.StringTable[IntentDescriptionKey]
			.Replace("-", CalculateAmountWithContext(context).ToString());
	}
}
