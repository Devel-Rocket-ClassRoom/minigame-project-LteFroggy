using UnityEngine;

public abstract class EnemyAction : ActionBase<EnemyActionContext> {
	public abstract string IntentIconName { get; }
	// 적 의도 아이콘 주소
	public Sprite IntentIcon => Resources.Load<Sprite>($"Sprites/Intents/{IntentIconName}");
	// 적 의도 아이콘 밑에 표시될 텍스트
	public abstract string GetIntentTextWithContext(EnemyActionContext context);
}