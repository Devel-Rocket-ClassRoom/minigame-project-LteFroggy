using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public static class StringTableManager {
	private const string k_TablePath = "Datas/KorStringData";
	private static Dictionary<string, string> _stringTable = new();
	public static Dictionary<string, string> StringTable => _stringTable;
	
	static StringTableManager() {
		Load(k_TablePath);

	}
	
	private static void Load(string path) {
		TextAsset csvFile = Resources.Load<TextAsset>(path);
		if (csvFile == null) { Debug.LogError($"{k_TablePath} 파일이 없습니다.");}
		
		StringReader reader = new StringReader(csvFile.text);
		CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
		
		IEnumerable<DataRow> rows = csvReader.GetRecords<DataRow>();
		
		foreach (DataRow row in rows) {
			_stringTable.Add(row.id, row.value);
		}
	}
}