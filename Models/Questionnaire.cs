namespace EIAS.Models
{
    public class Questionnaire
    {
        // Household Properties
        public int HouseholdSize { get; set; }
        public decimal Electricity { get; set; }
        public decimal NaturalGas { get; set; }
        public decimal Coal { get; set; }
        public decimal Solar { get; set; }
        public decimal Petroleum { get; set; }
        public decimal Wind { get; set; }

        // Car Properties
        public int CarCount { get; set; }
        public string Manufacturer { get; set; }
        public int ModelYear { get; set; }
        public string CarModel { get; set; }
        public decimal MileageEfficiency { get; set; }
        public string FuelType { get; set; }

        // Travel Properties
        public string PublicTransportationType { get; set; }
        public int FrequencyPerMonth { get; set; }
        public string CommuteDuration { get; set; }
        public bool CommuteWithOthers { get; set; }
    }
}
