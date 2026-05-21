using UnityEngine;

[CreateAssetMenu(menuName = "Card/Card Actions/Give Weakness")]
public class WeaknessAction : CardAction {
	public int amount;
	public override int Amount => amount; 
	public override string CardDescriptionKey => "WeaknessCardText";
	
	// 취약 효과 부여
	public override void Execute(BattleContext context) {
		if (context.target.IsDead) return;
		
		var weakness = new Weakness();
		weakness.Init(context.target, 0, Amount);
		context.target.AddStatus(weakness);
	}
	
	public override int CalculateAmountWithContext(BattleContext context) {
		return Amount;
	}
}