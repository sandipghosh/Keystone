
namespace Keystone.Web.Models
{
    public enum StatusEnum
    {
        Active = 1,
        Inactive = 2
    }

    public enum DeliveryScheduleEnum
    {
        StandardTurnaround = 1,
        ExpeditedTurnaround = 2
    }

    public enum AddressTypeEnum
    {
        Shipping = 1,
        Billing = 2
    }

    public enum PaymentTypeEnum
    {
        CreditCard = 1,
        Paypal = 2
    }

    public enum AttachmentTypeEnum
    {
        PDF,
        Archive
    }

    public enum DisplayProperty
    {
        Name,
        Description
    }
}