using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SqlServerEFCore
{
    public class Test1Context : DbContext
    {
        public Test1Context(DbContextOptions<Test1Context> options) : base(options)
        {

        }

        public DbSet<Test1Data> Test1Data { get; set; }
    }

    public class Test1Data
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Data { get; set; }
    }
}
