using System;
using System.Collections.Generic;
using UnityEngine;
namespace Importer.Core.DynamicData
{
    /// <summary>
    ///     ScriptableObject that defines the schema for a CSV import.
    ///     Designers can create instances of this to describe the expected columns and their types.
    /// </summary>
    [CreateAssetMenu(fileName = "DataSchema", menuName = "Importer/Data Schema")]
    // ReSharper disable once InconsistentNaming
    public class DataSchemaSO : ScriptableObject
    {
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        private List<ColumnDefinition> columns = new List<ColumnDefinition>();

        public List<ColumnDefinition> Columns => columns;

        public ColumnDefinition GetColumn(string columnName)
        {
            foreach (ColumnDefinition column in Columns)
            {
                if (string.Equals(column.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return null;
        }
    }
}
