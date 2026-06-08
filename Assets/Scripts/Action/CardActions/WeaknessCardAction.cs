using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Give Weakness")]
public class WeaknessCardAction : CardAction {
	public int amount;
	protected override int Amount => amount; 
	public override string CardDescriptionKey => "WeaknessCardText";
	
	// 취약 효과 부여
	public override void Execute(CardUseContext context) {
		if (context.target.IsDead) return;
		
		var weakness = new Weakness();
		weakness.Init(context.target, 0, ApplyAmountWithContext(context));
		context.target.AddStatus(weakness);
	}
	
	protected override int CalculateAmountWithContext(CardUseContext context, CalculationMode mode) {
		return context.relicManager.CalculateAmountWithRelics(context, this, Amount);
	}
}
