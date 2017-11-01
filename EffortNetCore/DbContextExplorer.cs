using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata;

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

                newEntity.FieldMapping = HandleFieldMapping(entity);

                contextDef.AllTables.Add(tableName, newEntity);
            });

            return contextDef;
        }

        private Dictionary<string, EntityFieldDefinition> HandleFieldMapping(IEntityType entity)
        {
            var mapping = new Dictionary<string, EntityFieldDefinition>();
            var foreignKeys = entity.GetForeignKeys().ToList();
            foreach (var property in entity.ClrType.GetProperties())
            {
                var foreignKey = foreignKeys.Where(fk => fk.DeclaringEntityType == entity
                    && fk.Properties[0].IsShadowProperty
                    && fk.DependentToPrincipal.Name == property.Name).FirstOrDefault();
                if (foreignKey != null)
                {
                    mapping.Add(foreignKey.Properties[0].Name,
                        new EntityFieldDefinition
                        {
                            Name = property.Name,
                            PrincipalEntityClrType = foreignKey.PrincipalEntityType.ClrType,
                            IsDependantAndShadowKey = true
                        });
                }
                else
                {
                    mapping.Add(property.Name, new EntityFieldDefinition { Name = property.Name });
                }
            }

            return mapping;
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
        public Dictionary<string, EntityFieldDefinition> FieldMapping { get; set; }
        public string DbSetName { get; set; }
    }

    public class EntityFieldDefinition
    {
        public string Name { get; set; }
        public bool IsDependantAndShadowKey { get; set; }
        public Type PrincipalEntityClrType { get; set; }
    }
}