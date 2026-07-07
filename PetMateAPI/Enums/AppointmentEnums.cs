namespace PetMateAPI.Enums
{

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

    public enum AppointmentStatus
    {
        Pending = 0,
        Completed = 2,
        CancelledByUser = 3,
        CancelledByVet = 4,
        CancelledByAdmin = 5
    }

}
