public interface IHasHealth {
	public int MaxHealth { get; }
	public int CurrentHealth { get; }
	public void GetDamage(int amount);
	public void GetHeal(int amount);
}