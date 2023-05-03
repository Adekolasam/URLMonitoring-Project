using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace URLMonitoring_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<pParam> pParams { get; set; }
        public DbSet<Environment> Environments { get; set; }
        public DbSet<EnvironmentVar> EnvironmentVars { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<UrlGroup> UrlGroups { get; set; }
        public DbSet<Url> Urls { get; set; }
    }

    public class Environment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<EnvironmentVar> EnvironmentVars { get; set; }
        public Collection Collection { get; set; }
    }

    public class EnvironmentVar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int EnvironmentId { get; set; }
        [Required]
        public string Variable { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Value { get; set; }
        [ForeignKey("EnvironmentId")]
        public Environment environment { get; set; }

        [ForeignKey("TypeId")]
        public pParam pParam { get; set; }
    }

    public class pParam
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is 100 characters.")]
        public string Code { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Maximum descrption length is 300 characters.")]
        public string Description { get; set; }

        public ICollection<EnvironmentVar> EnvironmentVars { get; set; }
    }

    public class Collection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? EnvironmentId { get; set; }

        [ForeignKey("EnvironmentId")]
        public Environment Environment { get; set; }
        public ICollection<UrlGroup> UrlGroups { get; set; }
        public ICollection<Url> Urls { get; set; }
    }

    public class UrlGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CollectionId { get; set; }

        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; }

        public ICollection<Url> Urls { get; set; }
    }

    public class Url
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string Path { get; set; }
        [Required]
        public int CollectionId { get; set; }

        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; }

        public int? UrlGroupId { get; set; }

        [ForeignKey("UrlGroupId")]
        public UrlGroup UrlGroup { get; set; }
        public int RequestType { get; set; }
        public bool ActiveStatus { get; set; }
        //public int schedulerId { get; set; }
    }

}
