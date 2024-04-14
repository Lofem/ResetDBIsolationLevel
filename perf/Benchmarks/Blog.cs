using System.ComponentModel.DataAnnotations;

namespace ResetDBIsolationLevel.Benchmarks;

public class Blog
{
    [Key]
    public int Id { get; set; }
}