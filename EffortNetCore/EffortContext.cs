using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EffortNetCore
{
    public class EffortContext<T> where T : DbContext
    {
        private string _pathToFolder;
        private T _context;

        public EffortContext(string pathToExcelFolder)
        {
            _pathToFolder = pathToExcelFolder;

            var options = new DbContextOptionsBuilder<T>().UseInMemoryDatabase().Options;

            Type contextType = typeof(T);
            CheckTemplateTypeHasRightParameter(contextType);

            _context = (T)Activator.CreateInstance(contextType, new[] { options });
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            GenerateModelFromFiles();
            _context.SaveChanges();
        }

        private static void CheckTemplateTypeHasRightParameter(Type contextType)
        {
            ConstructorInfo[] ci = contextType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var lookedForType = typeof(DbContextOptions<T>);
            var hasRightConstructor = false;
            foreach (var constructor in ci)
            {
                var parameters = constructor.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.FullName == lookedForType.FullName)
                {
                    hasRightConstructor = true;
                    break;
                }
            }
            if (!hasRightConstructor)
            {
                throw new EffortException("No suitable constructor found");
            }
        }

        private void GenerateModelFromFiles()
        {
            if (!Directory.Exists(_pathToFolder))
            {
                throw new EffortException("No directory found at location " + _pathToFolder);
            }

            var contextExplorer = new DbContextExplorer(_context);
            var contextDef = contextExplorer.ReadContext();

            var parser = new ExcelDatabaseParser<T>(_pathToFolder, _context, contextDef);
            parser.ParseAll();
        }

        public T GetContext()
        {
            return _context;
        }
    }
}