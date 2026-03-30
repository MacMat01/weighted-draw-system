# Dynamic Data Importer

This guide is written for two audiences:

- **Designers** who want to configure and use the importer without touching code.
- **Developers** who may maintain or extend parser behavior.

The importer lives in `Assets/Scripts/Importer/Core/DynamicData` and converts CSV/JSON into typed `DataRecord` objects.

## Start Here

If your goal is "import my data and use it in gameplay," follow this path.

### 1) Create and configure a schema

1. Create a `DataSchemaSO` asset (`Create > Importer > Data Schema`).
2. Add entries in `Columns`.
3. For each entry:
   - set `ColumnName` to match your CSV header or JSON property name
   - set `DataType` (`String`, `Int`, `Float`, `Bool`, `ConditionList`)
   - enable `IsRequired` for fields that must exist and must not be empty
4. Assign your source `TextAsset` to `SourceDataFile`.

### 2) Author source files correctly

- CSV headers are matched case-insensitively.
- JSON properties are matched case-insensitively.
- Extra columns/properties not listed in schema are ignored.
- Missing optional fields are allowed.
- Missing/empty required fields cause that row/object to be skipped.

### 3) Import data

Typical import call:

```csharp
List<DataRecord> records = DynamicDataImporter.ImportFromSchema(schema);
```

### 4) Understand common outcomes

- If import returns fewer rows than expected, check Console logs first.
- Warnings usually mean a value could not be converted and default was used.
- Errors on required fields mean that row/object is rejected.

## Behavior Overview

1. `DataSchemaSO` defines expected columns, types, and required flags.
2. `DynamicDataImporter` selects CSV vs JSON by extension (or by content sniffing when extension is missing).
3. Parser converts each schema column to a typed value.
4. Each accepted row/object becomes a `DataRecord`.
5. `ConditionList` values become `List<ParsedCondition>`.

Rows/objects that fail required-field checks are skipped.

## Data Types and Conversion Rules

### `String`

- CSV: value is trimmed.
- JSON: raw parsed string value is used (can be `null`).

### `Int`

- Parsed with invariant culture.
- Invalid value logs warning and defaults to `0`.

### `Float`

- Parsed with invariant culture.
- Invalid value logs warning and defaults to `0f`.

### `Bool`

- Accepts `true`/`false` and `1`/`0`.
- Invalid value logs warning and defaults to `false`.

### `ConditionList`

- Parsed by `ConditionParserUtility` into `List<ParsedCondition>`.
- Empty/null condition text returns an empty list.

## Entry Points

### `DynamicDataImporter`

### `ImportRaw(string rawText, string extension, DataSchemaSO schema)`

- Returns empty list if `rawText` is blank or `schema` is null.
- Normalizes extension:
  - explicit extension wins (`csv`, `.csv`, `json`, `.json`)
  - no extension + leading `{`/`[` => JSON
  - otherwise => CSV
- Routes to `SchemaDrivenCsvParser.Parse(...)` or `SchemaDrivenJsonParser.Parse(...)`.
- Unsupported extension logs warning and returns empty list.

### `ImportFromSchema(DataSchemaSO schema)`

- Throws `ArgumentNullException` if schema is null.
- Throws `InvalidOperationException` if `SourceDataFile` is missing.
- Uses `sourceDataFile.name` extension and `sourceDataFile.text` content.

### `ImportFromTextAsset(TextAsset textAsset, DataSchemaSO schema)`

- Returns empty list if `textAsset` is null.
- Uses `textAsset.name` and `textAsset.text`.
- Null schema returns empty list via `ImportRaw(...)`.

### `ImportFromFilePath(string filePath, DataSchemaSO schema)`

- Returns empty list if path is empty or missing on disk.
- Reads file then routes through `ImportRaw(...)`.
- Null schema returns empty list via `ImportRaw(...)`.

## Parser Details

### CSV (`SchemaDrivenCsvParser`)

- Handles quoted commas, escaped quotes (`""`), multiline quoted fields, LF/CRLF.
- First non-empty record is treated as header row.
- Missing schema header logs warning once per missing column.
- Missing/empty required values log error and invalidate row.
- Invalid row is skipped and logs warning (`Skipping row ...`).

### JSON (`SchemaDrivenJsonParser`)

- Supports object root (`{...}`) and array root (`[...]`).
- Other root forms are rejected with warning.
- Required checks:
  - missing required key => invalid object
  - null/empty/whitespace on required key => invalid object
- Invalid object is skipped and logs warning (`Skipping item ...`).

### Nested JSON alias expansion

JSON parser expands nested objects into flat aliases so schemas can stay flat.

Examples of generated aliases include:

- `Left_Choice_Answer`
- `Left_Answer`
- `Left_Attribute1`, `Left_Attribute2`, ...

This supports card-like payloads with nested choice/attribute objects.

## Condition Syntax (`ConditionParserUtility`)

Supported operators:

- `==`, `!=`, `>=`, `<=`, `>`, `<`

Supported connectors/separators:

- `&&`, `||`, `and`, `or`, `&`, `|`, `;`

Connector normalization:

- `AND`: `&&`, `&`, `and`, `;`
- `OR`: `||`, `|`, `or`

Supported shorthand:

- `alchemy` => `alchemy == 1`
- `!cursed` => `cursed != 1`

Supported literals:

- `TRUE`, `FALSE` (case-insensitive)

Robustness behavior:

- Empty/null/whitespace input => empty condition list
- Malformed segments are skipped
- Valid segments around malformed segments are preserved
- Connector metadata starts from the second parsed condition

## Troubleshooting

- **Rows missing after import**: check required fields and empty values first.
- **Unexpected zeros/false**: source value likely failed type parse and defaulted.
- **Condition not working**: verify operator/connectors and variable naming format.
- **JSON nested fields not mapping**: confirm schema column uses one of the generated alias forms.

## Source of Truth (Tests)

Current behavior is covered in:

- `Assets/Tests/EditMode/Importer/Core/DynamicData/SchemaDrivenCsvParserTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/SchemaDrivenJsonParserTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/DynamicDataImporterCsvIntegrationTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/DynamicDataImporterJsonIntegrationTests.cs`

Notable covered scenarios:

- CSV quoting/escaping, multiline fields, CRLF, blank row skipping
- JSON object array and single-object roots
- required vs optional behavior, including whitespace-only required failures
- parse-default behavior for invalid numbers/bools
- condition operators/connectors/flags/literals and malformed segment tolerance
- nested JSON alias mapping to flat schema columns

Current test scope is parser behavior plus `ImportFromSchema(...)` integration using example schema assets.

