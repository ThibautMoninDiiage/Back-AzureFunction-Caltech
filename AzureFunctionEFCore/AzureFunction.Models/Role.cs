using AzureFunction.Models.BaseModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureFunction.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        public string? Name { get; set; }
    }
}
