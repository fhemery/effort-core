using System.ComponentModel.DataAnnotations.Schema;

namespace EffortNetCore.Tests.context.model
{
    [Table("AnotherTable")]
    internal class ObjectWithOnlyAnIdAndTableName
    {
        public int Id { get; set; }
    }
}