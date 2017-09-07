using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EffortNetCore
{
    public class DbContextExplorer
    {
        private DbContext _context;

        public DbContextExplorer(DbContext context)
        {
            _context = context;
        }

        public ContextDefinition ReadContext()
        {
            var contextDef = new ContextDefinition();

            _context.Model.GetEntityTypes().ToList().ForEach(entity =>
            {
                var newEntity = new EntityDefinition { EntityName = entity.Name, EntityClrType = entity.ClrType };

                // Get its DbSet
                var allDbSets = _context.GetType().GetProperties().Where(p => p.PropertyType.FullName.Contains("DbSet")).ToList();
                var dbSet = allDbSets.SingleOrDefault(p => p.PropertyType.GenericTypeArguments.Length == 1 && p.PropertyType.GenericTypeArguments[0].FullName == entity.Name);
                if (dbSet != null)
                {
                    newEntity.DbSetName = dbSet.Name;
                }

                var tableName = entity.ClrType.Name;
                var allAnnotations = entity.GetAnnotations();
                var tableAttribute = entity.ClrType.GetTypeInfo().GetCustomAttribute<TableAttribute>(true);
                if (tableAttribute != null)
                {
                    tableName = tableAttribute.Name;
                }
                else if (newEntity.DbSetName != null)
                {
                    tableName = newEntity.DbSetName;
                }

                contextDef.AllTables.Add(tableName, newEntity);
            });

            return contextDef;
        }
    }

    public class ContextDefinition
    {
        public Dictionary<string, EntityDefinition> AllTables { get; set; }

        public ContextDefinition()
        {
            AllTables = new Dictionary<string, EntityDefinition>();
        }
    }

    public class EntityDefinition
    {
        public string EntityName { get; set; }
        public Type EntityClrType { get; set; }
        public Dictionary<string, string> FieldMapping { get; set; }
        public string DbSetName { get; set; }
    }
}