using System.ComponentModel.DataAnnotations.Schema;

namespace AzureFunction.Context.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        public string? Name { get; set; }
    }
}