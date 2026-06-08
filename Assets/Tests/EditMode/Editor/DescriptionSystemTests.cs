using NUnit.Framework;

public class DescriptionSystemTests {
	private const string TestKeyword = "테스트키워드";
	private const string TestDescription = "테스트 설명";

	[Test]
	public void ProcessTextWrapsKeywordsWithBlueHighlightColor() {
		bool hadOriginal = StringTableManager.DescriptionTable.TryGetValue(TestKeyword, out string originalDescription);
		StringTableManager.DescriptionTable[TestKeyword] = TestDescription;

		try {
			string processed = DescriptionSystem.ProcessText($"카드 설명에 {TestKeyword}가 포함됩니다.");

			Assert.That(processed, Does.Contain($"<color=#2563EB>{TestKeyword}</color>"));
		}
		finally {
			if (hadOriginal) {
				StringTableManager.DescriptionTable[TestKeyword] = originalDescription;
			}
			else {
				StringTableManager.DescriptionTable.Remove(TestKeyword);
			}
		}
	}
}
