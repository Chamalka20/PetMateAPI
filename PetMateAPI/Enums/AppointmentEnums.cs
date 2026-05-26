namespace PetMateAPI.Enums
{
    public enum AppointmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        Completed = 2,
        Cancelled = 3,
        NoShow = 4
    }

    public enum AppointmentType
    {
        ClinicVisit = 0,
        HomeVisit = 1,
        Emergency = 2,
        Virtual = 3
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Refunded = 2,
        Failed = 3
    }
}
