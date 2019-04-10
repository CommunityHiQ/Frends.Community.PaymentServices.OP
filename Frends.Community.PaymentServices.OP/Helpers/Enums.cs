namespace Frends.Community.PaymentServices.OP.Helpers
{
    public class Enums
    {
        public enum Environment
        {
            TEST,
            PRODUCTION
        }
        // Possible statuses for files sent by the customer 
        // WFP = Waiting for processing
        // FWD = Forwarded
        // Possible statuses for files that are fetchable by the customer
        // NEW = New (not yet downloaded)
        // DLD = Downloaded
        public enum Status
        {
            WFP,
            FWD,
            NEW,
            DLD,
            ALL
        }
    }
}
