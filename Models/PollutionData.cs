using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BitirmeProjesi.Models
{
    public class PollutionData
    {
        public int Id { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string MetalType { get; set; }
        public float Value { get; set; }
        public DateTime DataRecorded { get; set; }
        public string EnteredById { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public virtual IdentityUser EnteredByUser { get; set; }
    }
}