using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public static class StringTableManager {
	private const string k_StringTablePath = "Datas/StringTables/KorStringData";
	private const string k_CardNameTablePath = "Datas/StringTables/KorCardData";
	private const string k_DescriptionTablePath = "Datas/StringTables/KorDescriptionData";
	private static Dictionary<string, string> _stringTable = new();
	public static Dictionary<string, string> StringTable => _stringTable;
	private static Dictionary<string, string> _cardNameTable = new();
	public static Dictionary<string, string> CardNameTable => _cardNameTable;
	private static Dictionary<string, string> _descriptionTable = new();
	public static Dictionary<string, string> DescriptionTable => _descriptionTable;

	static StringTableManager() {
		Load(k_StringTablePath, _stringTable);
		Load(k_CardNameTablePath, _cardNameTable);
		Load(k_DescriptionTablePath, _descriptionTable);
	}
	
	private static void Load(string path, Dictionary<string, string> table) {
		table.Clear();
		
		TextAsset csvFile = Resources.Load<TextAsset>(path);
		if (csvFile == null) { Debug.LogError($"{k_StringTablePath} 파일이 없습니다.");}
		
		StringReader reader = new StringReader(csvFile.text);
		CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
		
		IEnumerable<DataRow> rows = csvReader.GetRecords<DataRow>();
		
		foreach (DataRow row in rows) {
			table.Add(row.id, row.value);
		}
	}
}