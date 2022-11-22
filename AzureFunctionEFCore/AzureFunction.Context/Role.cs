using System.ComponentModel.DataAnnotations.Schema;
using AzureFunction.Models.BaseModels;

namespace AzureFunction.Models
{
    [Table("Roles")]
    public class Role : BaseModel
    {
        public string? Name { get; set; }
    }
}