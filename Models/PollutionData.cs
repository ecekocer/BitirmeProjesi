namespace BitirmeProjesi.Models
{
    public class PollutionData
    {
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string MetalType { get; set; }
        public DateTime DataRecorded { get; set; }
        public double Value { get; set; }
        public string EnteredBy { get; set; }
    }
}