namespace SBReceiverAF
{
    public class PlannedOrder
    {
        public string PlannedOrderNumber { get; set; }
        public string Material { get; set; }
        public string PlanningPlant { get; set; }
        public string TotalPlordQty { get; set; }
        public string BaseUom { get; set; }
        public string OrderStartDate { get; set; }
        public string OrderEndDate { get; set; }
        public string Version { get; set; }
        public string Line { get; set; }
        public bool FirmingIndicator { get; set; }
        public string PlannedOrderLastChangeDate { get; set; }
    }
}
