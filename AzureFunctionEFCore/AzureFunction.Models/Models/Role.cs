using AzureFunction.Models.Models.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureFunction.Models.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        public string? Name { get; set; }
    }
}
