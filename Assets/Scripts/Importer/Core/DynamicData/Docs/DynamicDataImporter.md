# Dynamic Data Importer

This document explains how the importer in `Assets/Scripts/Importer/Core/DynamicData` reads CSV/JSON text and turns it into typed `DataRecord` objects.

## Overview

The importer is schema-driven:

1. A `DataSchemaSO` defines expected columns and types.
2. `DynamicDataImporter` chooses CSV or JSON parsing based on extension (or content sniffing).
3. The selected parser converts raw values into typed fields.
4. Each row/object becomes one `DataRecord`.
5. `ConditionList` fields are parsed into `List<ParsedCondition>`.

## Main Types

- `DataSchemaSO`: ScriptableObject holding:
  - `columns` (`List<ColumnDefinition>`)
  - `sourceDataFile` (`TextAsset`)
- `ColumnDefinition`: `{ ColumnName, DataType }`
- `ColumnDataType`: `String`, `Int`, `Float`, `Bool`, `ConditionList`
- `DataRecord`: dynamic dictionary-like container (`SetField`, `GetField`)
- `ParsedCondition`: structured condition item with optional connector metadata

## Entry Points (`DynamicDataImporter`)

### `ImportRaw(string rawText, string extension, DataSchemaSO schema)`

- Returns empty list when text is blank or schema is null.
- Normalizes extension:
  - Explicit extension wins (`csv`, `.csv`, `json`, `.json`)
  - If extension missing: text starting with `{` or `[` is treated as JSON
  - Otherwise treated as CSV
- Routes to:
  - `SchemaDrivenCsvParser.Parse(...)`
  - `SchemaDrivenJsonParser.Parse(...)`
- Unsupported extension logs warning and returns empty list.

### `ImportFromSchema(DataSchemaSO schema)`

- Throws `ArgumentNullException` if schema is null.
- Throws `InvalidOperationException` if schema has no `sourceDataFile`.
- Uses `sourceDataFile.name` extension and `sourceDataFile.text` as input.

### `ImportFromTextAsset(TextAsset textAsset, DataSchemaSO schema)`

- Returns empty list when `textAsset` is null.
- Uses asset name extension and text content.

### `ImportFromFilePath(string filePath, DataSchemaSO schema)`

- Returns empty list when path is blank or file does not exist.
- Reads file text from disk and routes via extension.

## CSV Parsing (`SchemaDrivenCsvParser`)

CSV parser behavior:

- Splits records while respecting quoted fields.
- Supports:
  - CRLF and LF line endings
  - blank line skipping
  - escaped quotes (`""`) inside quoted fields
  - commas and newlines inside quoted fields
- Uses first record as header row.
- Header matching is case-insensitive.
- For each schema column:
  - If header missing: logs warning once per missing column name and skips it.
  - If cell exists: converts value based on `ColumnDataType`.

Type conversion rules:

- `String`: trimmed string
- `Int`: `int.TryParse` (InvariantCulture), default `0` on failure
- `Float`: `float.TryParse` (InvariantCulture), default `0f` on failure
- `Bool`: `bool.TryParse`; also accepts `0`/`1`; default `false` on failure
- `ConditionList`: parsed by `ConditionParserUtility.Parse`

## JSON Parsing (`SchemaDrivenJsonParser`)

JSON parser behavior:

- Accepts root as:
  - array of objects (`[...]`)
  - single object (`{...}`)
- Other root forms are rejected with warning.
- Uses a lightweight parser to:
  - split top-level objects in arrays
  - parse object properties case-insensitively
  - decode common escaped characters in strings
- Only schema-defined keys are imported.
- Missing schema keys are left unset (`GetField(...)` returns `null`).
- Extra JSON keys are ignored.

Type conversion rules match CSV defaults:

- Invalid `Int` -> `0`
- Invalid `Float` -> `0f`
- Invalid `Bool` -> `false`
- `ConditionList` uses `ConditionParserUtility.Parse`
- JSON `null` values are converted to empty string before conversion (for `ConditionList`, this yields an empty list)

## Condition Syntax (`ConditionParserUtility`)

Supported operators:

- `==`, `!=`, `>=`, `<=`, `>`, `<`

Supported connectors/separators:

- `&&`, `||`, `and`, `or`, `&`, `|`, `;`

Connector normalization:

- `AND`: `&&`, `&`, `and`, `;`
- `OR`: `||`, `|`, `or`

Boolean flag shorthand:

- `alchemy` -> `alchemy == 1`
- `!cursed` -> `cursed != 1`

Robustness details:

- Empty/null/whitespace input returns empty list.
- Malformed segments are skipped.
- Valid segments around malformed ones are still kept.
- Leading/trailing connectors are tolerated.
- First parsed condition has no connector metadata.

## Typical Usage

1. Create a `DataSchemaSO` asset.
2. Add column definitions with names matching CSV headers or JSON property names.
3. Assign a CSV/JSON `TextAsset` to `sourceDataFile`.
4. Import:

```csharp
List<DataRecord> records = DynamicDataImporter.ImportFromSchema(schema);
```

Or with raw text:

```csharp
List<DataRecord> records = DynamicDataImporter.ImportRaw(rawText, ".csv", schema);
```

## Verified Behaviors (Tests)

Behavior examples are covered in:

- `Assets/Tests/EditMode/Importer/Core/DynamicData/DataRecordConditionParsingTests.cs`

Notable test cases include:

- Extension routing and JSON auto-detection
- CSV quoted commas, escaped quotes, multiline fields, CRLF, blank lines
- Typed conversion defaults for invalid values
- Condition operators/connectors/flags and malformed segment handling
- `ImportFromSchema` exception behavior when source file is missing

