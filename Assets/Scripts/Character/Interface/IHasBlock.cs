public interface IHasBlock {
    int Block { get; }
    void AddBlock(int amount);
    void LoseBlock(int amount);
    void ClearBlock();
}
