using System.ComponentModel.DataAnnotations;
using URLMonitoring_API.Data;
using URLMonitoring_API.Repo;

namespace URLMonitoring_API.DTOs
{
    public class DeletionIds
    {
        public int[] Ids { get; set; }
    }
    public class EnvironmentDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public Collection Collection { get; set; }
    }

    public class EnvironmentReadDTO
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool Deleteable { get; set; }
    }

    public class EnvironmentPostDTO
    {
        [Required]
        public string Name { get; set; }
    }

    public class EnvironmentVarDTO
    {
        //non encryptedtypes
        public int Id { get; set; }
        [Required]
        public int EnvironmentId { get; set; }
        [Required]
        public string Variable { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Value { get; set; }
        public string EnvironmentName { get; set; }
        public string TypeName { get; set; }
        public bool Deleteable { get; set; }
        //public Data.Environment Environment { get; set; }
        //public pParam PParam{ get; set; }

    }

    public class EnvOptPostVarDTO
    {
        public int Id { get; set; }
        [Required]
        public int EnvironmentId { get; set; }
        [Required]
        public string Variable { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Value { get; set; }
    }


    public class EnvironmentPutVarDTO
    {
        public int Id { get; set; }
        [Required]
        public string Variable { get; set; }
        [Required]
        public int TypeId { get; set; }
        [Required]
        public string Value { get; set; }
    }

    public class CollectionDTO
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? EnvironmentId { get; set; }
        public EnvironmentDTO Environment { get; set; }
        //public ICollection<UrlGroup> UrlGroups { get; set; }
        //public ICollection<Url> Urls { get; set; }
    }

    public class EnvVarTypeDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is 100 characters.")]
        public string Code { get; set; }
        [Required]
        [StringLength(300, ErrorMessage = "Maximum descrption length is 300 characters.")]
        public string Description { get; set; }

    }

    public class pParamDTO
    {
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

    }
}
