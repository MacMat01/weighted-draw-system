using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Data
{
    public static class UniversalImporter
    {
        private static readonly Dictionary<string, IDataParser> ParsersByExtension = new Dictionary<string, IDataParser>(StringComparer.OrdinalIgnoreCase)
        {
            {
                ".csv", new CsvDataParser()
            },
            {
                ".json", new JsonDataParser()
            }
        };

        public static List<T> ImportFromTextAsset<T>(TextAsset textAsset) where T : new()
        {
            if (textAsset == null)
            {
                Debug.LogWarning("UniversalImporter: TextAsset is null.");
                return new List<T>();
            }

            string extension = Path.GetExtension(textAsset.name);
            if (string.IsNullOrEmpty(extension))
            {
                extension = GuessExtension(textAsset.text);
            }

            return ImportRawText<T>(textAsset.text, extension);
        }

        public static List<T> ImportFromFilePath<T>(string filePath) where T : new()
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                Debug.LogWarning("UniversalImporter: File path is empty.");
                return new List<T>();
            }

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"UniversalImporter: File not found at '{filePath}'.");
                return new List<T>();
            }

            string rawText = File.ReadAllText(filePath);
            string extension = Path.GetExtension(filePath);
            return ImportRawText<T>(rawText, extension);
        }

        public static List<T> ImportRawText<T>(string rawText, string extension) where T : new()
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return new List<T>();
            }

            string normalizedExtension = NormalizeExtension(extension, rawText);
            if (!ParsersByExtension.TryGetValue(normalizedExtension, out IDataParser parser))
            {
                Debug.LogWarning($"UniversalImporter: No parser registered for extension '{normalizedExtension}'.");
                return new List<T>();
            }

            return parser.Parse<T>(rawText);
        }

        private static string NormalizeExtension(string extension, string rawText)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                return extension.StartsWith(".", StringComparison.Ordinal) ? extension : "." + extension;
            }

            return GuessExtension(rawText);
        }

        private static string GuessExtension(string rawText)
        {
            string trimmed = rawText?.TrimStart() ?? string.Empty;
            if (trimmed.StartsWith("[", StringComparison.Ordinal) || trimmed.StartsWith("{", StringComparison.Ordinal))
            {
                return ".json";
            }

            return ".csv";
        }
    }
}
