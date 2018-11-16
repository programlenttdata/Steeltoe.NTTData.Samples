using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlServerEFCore
{
    public class Test2Context : DbContext
    {
        public Test2Context(DbContextOptions<Test2Context> options) : base(options)
        {

        }

        public DbSet<Test2Data> Test2Data { get; set; }
    }

    public class Test2Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Data { get; set; }
    }
}
