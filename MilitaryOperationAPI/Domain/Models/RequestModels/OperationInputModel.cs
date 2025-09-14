using MilitaryOperationAPI.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MilitaryOperationAPI.Domain.Models.RequestModels
{
    public class OperationInputModel
    {
        public string? Description { get; set; }
        [Required]
        public string Reason { get; set; }
        public ActionTypeEnum ActionType { get; set; }
    }
}
