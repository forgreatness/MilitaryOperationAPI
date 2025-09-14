using MilitaryOperationAPI.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MilitaryOperationAPI.Domain.Models.Entities
{
    public class Operation
    {
        public Guid OperationID { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Reason { get; set; }
        public ActionTypeEnum ActionType { get; set; }
        [ForeignKey(nameof(AssignedByUser))]
        public Guid AssignedByUserID { get; set; }
        public User? AssignedByUser { get; set; }
        public Operation(DateTime startDate, string reason, ActionTypeEnum actionType, Guid assignedByUserID)
        {
            StartDate = startDate;
            Reason = reason;
            ActionType = actionType;
            AssignedByUserID = assignedByUserID;
        }
    }
}
