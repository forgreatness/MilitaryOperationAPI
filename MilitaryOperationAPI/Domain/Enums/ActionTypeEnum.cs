using System.ComponentModel;

namespace MilitaryOperationAPI.Domain.Enums
{
    public enum ActionTypeEnum
    {
        [Description("Nuke")]
        Nuke,

        [Description("Invade")]
        Invade,

        [Description("Assassinate")]
        Assassinate,

        [Description("Diplomacy")]
        Diplomacy,

        [Description("Defend")]
        Defend,

        [Description("Surrender")]
        Surrender
    }
}