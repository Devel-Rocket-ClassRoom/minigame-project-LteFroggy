using UnityEngine;

public class Overload : StatusBase {
	public override string IconName => "OverloadIcon";
	public override string DescriptionContent => base.DescriptionContent.Replace("1", Stack.ToString());
	public override string TextToShow => Stack.ToString();
	public override bool IsActive => Stack > 0;

	public override void Merge(StatusBase status) {
		Stack += status.Stack;
	}

	public override void OnTurnEnd() { }

	public override int ModifyStartingEnergy(int energy) {
		energy = Mathf.Max(0, energy - Stack);
		Stack = 0;
		return energy;
	}
}
