# Dynamic Data Importer

This guide reflects the current behavior implemented under `Assets/Scripts/Importer/Core/DynamicData`.

It is written for:

- **Designers** configuring schema assets and source files
- **Developers** maintaining parsing behavior

The importer converts CSV/JSON text into typed `DataRecord` rows using `DataSchemaSO`.

## Quick Start

### 1) Create and configure a schema

1. Create a `DataSchemaSO` asset (`Create > Importer > Data Schema`).
2. Add entries in `Columns`.
3. For each entry:
   - set `ColumnName` to match your CSV header or JSON property name
   - set `DataType` (`String`, `Int`, `Float`, `Bool`, `ConditionList`)
   - enable `IsRequired` for fields that must exist and must not be empty
4. Assign a source `TextAsset` to `SourceDataFile`.

### 2) Prepare your data

- CSV/JSON key matching is case-insensitive.
- Extra source fields not listed in schema are ignored.
- Missing optional fields are allowed.
- Missing or empty required fields reject that row/object.

### 3) Import

```csharp
List<DataRecord> records = DynamicDataImporter.ImportFromSchema(schema);
```

## Public API Surface

### `DynamicDataImporter.ImportFromSchema(DataSchemaSO schema)`

- Throws `ArgumentNullException` if `schema` is `null`.
- Throws `InvalidOperationException` if `schema.SourceDataFile` is missing.
- Uses `SourceDataFile.name` (extension) and `SourceDataFile.text` (content).

### `DynamicDataImporter.ImportFromTextAsset(TextAsset textAsset, DataSchemaSO schema)`

- Returns an empty list if `textAsset` is `null`.
- Uses `textAsset.name` and `textAsset.text`.
- If `schema` is `null`, import returns an empty list.

### `DynamicDataImporter.ImportFromFilePath(string filePath, DataSchemaSO schema)`

- Returns an empty list if `filePath` is blank or does not exist.
- Reads file text and routes to parser logic.
- If `schema` is `null`, import returns an empty list.

### Internal routing behavior

`DynamicDataImporter` uses a private raw import path that:

- returns empty if raw text is blank or schema is `null`
- normalizes extension:
  - explicit extension wins (`csv`, `.csv`, `json`, `.json`, etc.)
  - no extension + leading `{` or `[` => JSON
  - otherwise => CSV
- supports only `.csv` and `.json`
- logs warning and returns empty on unsupported extensions

## Parser Behavior

### CSV (`SchemaDrivenCsvParser`)

- Supports quoted commas, escaped quotes (`""`), multiline quoted fields, LF/CRLF.
- First non-empty record is used as header row.
- Header lookup is case-insensitive.
- Missing schema header logs one warning per missing schema column.
- Required checks per row:
  - required column missing from headers => row invalid
  - required cell missing/empty/whitespace => row invalid
- Invalid rows are skipped and log `Skipping row ...` warning.

### JSON (`SchemaDrivenJsonParser`)

- Supports root object (`{...}`) or root array (`[{...}]`).
- Any other root format logs warning and returns no records.
- Property lookup is case-insensitive.
- Required checks per object:
  - required key missing => object invalid
  - required value `null`/empty/whitespace => object invalid
- Invalid objects are skipped and log `Skipping item ...` warning.

### Nested JSON alias expansion

Nested JSON objects are flattened into additional alias keys so flat schemas can still bind nested data.

Examples of generated aliases include:

- full path aliases like `Left_Choice_Answer`
- common parent+leaf aliases like `Left_Answer`
- numbered aliases for grouped nested values like `Left_Attribute1`, `Left_Attribute2`

This supports card-like payloads where choices/attributes are nested objects.

## Type Conversion Rules

### `String`

- CSV: value is trimmed.
- JSON: parsed raw value is used as-is (can be `null`).

### `Int`

- Invariant-culture parse.
- Invalid parse logs warning and defaults to `0`.

### `Float`

- Invariant-culture parse.
- Invalid parse logs warning and defaults to `0f`.

### `Bool`

- Accepts `true`/`false` and `1`/`0`.
- Invalid parse logs warning and defaults to `false`.

### `ConditionList`

- Parsed by `ConditionParserUtility` into `List<ParsedCondition>`.
- Empty/null/whitespace source yields an empty list.

## Condition Syntax (`ConditionParserUtility`)

Supported operators:

- `==`, `!=`, `>=`, `<=`, `>`, `<`

Supported connectors/separators:

- `&&`, `||`, `and`, `or`, `&`, `|`, `;`

Connector normalization:

- `AND`: `&&`, `&`, `and`, `;`
- `OR`: `||`, `|`, `or`

Shorthand support:

- `alchemy` => `alchemy == 1`
- `!cursed` => `cursed != 1`

Literal constants:

- `TRUE`, `FALSE` (case-insensitive)

Robustness behavior:

- malformed segments are skipped
- valid segments around malformed ones are preserved
- connector metadata is assigned from the second parsed condition onward

## Practical Notes

- Fewer records than expected usually means required-field rejection.
- Unexpected `0`/`0f`/`false` values usually indicate parse fallback after warning.
- Condition issues are often malformed operators/connectors or invalid variable names.
- JSON nested binding issues are usually schema alias mismatches.

## Source of Truth (Tests)

Current behavior is covered by:

- `Assets/Tests/EditMode/Importer/Core/DynamicData/SchemaDrivenCsvParserTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/SchemaDrivenJsonParserTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/DynamicDataImporterCsvIntegrationTests.cs`
- `Assets/Tests/EditMode/Importer/Core/DynamicData/DynamicDataImporterJsonIntegrationTests.cs`

Notable covered scenarios include:

- CSV quoting/escaping, multiline fields, blank row skipping, CRLF support
- JSON root object/array handling and invalid root rejection
- optional vs required behavior, including whitespace-only required failures
- parse-default behavior for invalid numbers/bools
- condition operators/connectors/flags/literals and malformed segment tolerance
- nested JSON aliases mapped to flat schema fields

Integration test scope currently validates `ImportFromSchema(...)` against example schema assets.
