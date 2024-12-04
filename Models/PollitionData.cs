namespace BitirmeProjesi.Models
{
    public class PollitionData
    {
        public int Id { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string MetalType { get; set; }
        public DateTime DataRecorded { get; set; }
        public double Value { get; set; }
        public string EnteredBy { get; set; }
    }
}