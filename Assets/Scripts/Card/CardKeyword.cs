/// <summary>
/// 카드 인스턴스가 가지는 속성 키워드 정보.
/// 비트플래그로 여러 키워드를 동시에 가질 수 있으며, 런타임에 추가/제거 가능하다.
/// </summary>
public class CardKeyword {
	private CardKeywordType _flags;

	public CardKeyword(CardKeywordType initial = CardKeywordType.None) {
		_flags = initial;
	}

	public bool IsExhaust  => Has(CardKeywordType.Exhaust);
	public bool IsRetain   => Has(CardKeywordType.Retain);
	public bool IsOverload => Has(CardKeywordType.Overload);
	public bool IsReturn   => Has(CardKeywordType.Return);
	public bool IsInnate   => Has(CardKeywordType.Innate);
	public bool IsChain    => Has(CardKeywordType.Chain);

	public bool Has(CardKeywordType type)    => (_flags & type) != 0;
	public void Add(CardKeywordType type)    => _flags |= type;
	public void Remove(CardKeywordType type) => _flags &= ~type;
	public void Reset(CardKeywordType flags) => _flags = flags;
}
