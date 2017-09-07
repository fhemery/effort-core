using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EffortNetCore
{
    internal class ExcelDatabaseParser<T> where T : DbContext
    {
        private string _pathToFolder;
        private T _context;
        private ContextDefinition _contextDef;

        public ExcelDatabaseParser(string pathToFolder, T context, ContextDefinition contextDef)
        {
            _pathToFolder = pathToFolder;
            _context = context;
            _contextDef = contextDef;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public void ParseAll()
        {
            foreach (var filePath in Directory.GetFiles(_pathToFolder, "*.xlsx"))
            {
                var tableName = Path.GetFileNameWithoutExtension(filePath);
                if (!_contextDef.AllTables.ContainsKey(tableName))
                {
                    throw new EffortException("Table name " + tableName + " is not defined in DB. Please ensure DbSet and filename are valid");
                }

                var entityDetails = _contextDef.AllTables[tableName];

                if (entityDetails.DbSetName == null)
                {
                    throw new NotImplementedException("Not yet managing entities not having their dbsets. Come back later !");
                }

                Type contextType = typeof(T);
                var dbSetToAddTo = contextType.GetProperty(entityDetails.DbSetName).GetValue(_context);

                ReadFile(filePath, dbSetToAddTo, entityDetails.EntityClrType);
            }
        }

        private void ReadFile(string filePath, object dbSetToAddTo, Type entityType)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding("utf-8")
                }))
                {
                    string[] headers = ParseIndexes(reader);
                    var currentLine = 1;
                    while (reader.Read())
                    {
                        ++currentLine;
                        var newObject = Activator.CreateInstance(entityType);

                        for (var col = 0; col < headers.Length; ++col)
                        {
                            var targetProperty = entityType.GetProperty(headers[col]);

                            try
                            {
                                FillPropertyFromParsedField(reader, newObject, col, targetProperty);
                            }
                            catch (Exception e)
                            {
                                throw new EffortException($"Error occured when trying to convert line {currentLine}, column {col + 1}: " + e.Message);
                            }
                        }
                        dbSetToAddTo.GetType().GetMethod("Add").Invoke(dbSetToAddTo, new[] { newObject });
                    }
                }
            }
        }

        private void FillPropertyFromParsedField(IExcelDataReader reader, object newObject, int col, PropertyInfo targetProperty)
        {
            var fieldType = reader.GetFieldType(col);
            if (fieldType == typeof(string))
            {
                ParseString(reader.GetString(col), newObject, targetProperty);
            }
            else if (fieldType == typeof(double))
            {
                ParseDouble(reader.GetDouble(col), newObject, targetProperty);
            }
            else if (fieldType == typeof(int))
            {
                ParseInt(reader.GetInt32(col), newObject, targetProperty);
            }
            else if (fieldType == typeof(bool))
            {
                ParseBool(reader.GetBoolean(col), newObject, targetProperty);
            }
            // Only other possibility is that type is null. Do nothing in this case.
        }

        private void ParseBool(bool value, object newObject, PropertyInfo targetProperty)
        {
            targetProperty.SetValue(newObject, value);
        }

        private void ParseInt(int value, object newObject, PropertyInfo targetProperty)
        {
            if (targetProperty.PropertyType == typeof(int))
            {
                targetProperty.SetValue(newObject, value);
            }
            else if (targetProperty.PropertyType == typeof(bool))
            {
                targetProperty.SetValue(newObject, Convert.ToBoolean(targetProperty));
            }
            else if (targetProperty.PropertyType.GetTypeInfo().IsEnum)
            {
                targetProperty.SetValue(newObject, Convert.ToInt32(value));
            }
            else
            {
                throw new EffortException($"Received int {value} but couldn't cast it to {targetProperty.PropertyType.ToString()}");
            }
        }

        private void ParseDouble(double value, object newObject, PropertyInfo targetProperty)
        {
            if (targetProperty.PropertyType == typeof(int))
            {
                targetProperty.SetValue(newObject, Convert.ToInt32(value));
            }
            else if (targetProperty.PropertyType == typeof(double))
            {
                targetProperty.SetValue(newObject, value);
            }
            else if (targetProperty.PropertyType == typeof(bool))
            {
                targetProperty.SetValue(newObject, Convert.ToBoolean(value));
            }
            else if (targetProperty.PropertyType.GetTypeInfo().IsEnum)
            {
                targetProperty.SetValue(newObject, Convert.ToInt32(value));
            }
            else
            {
                throw new EffortException($"Received double {value} but couldn't cast it to {targetProperty.PropertyType.ToString()}");
            }
        }

        private void ParseString(string value, object newObject, PropertyInfo targetProperty)
        {
            if (targetProperty.PropertyType == typeof(Guid))
            {
                targetProperty.SetValue(newObject, new Guid(value));
            }
            else if (targetProperty.PropertyType == typeof(string))
            {
                targetProperty.SetValue(newObject, value);
            }
            else
            {
                throw new EffortException($"Received string {value} but couldn't cast it to {targetProperty.PropertyType.ToString()}");
            }
        }

        private string[] ParseIndexes(IExcelDataReader reader)
        {
            if (!reader.Read())
            {
                return new string[0];
            }

            var columnNames = new List<string>();
            for (int column = 0; column < reader.FieldCount; ++column)
            {
                columnNames.Add(reader.GetString(column));
            }

            return columnNames.ToArray();
        }
    }
}