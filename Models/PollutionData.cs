using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BitirmeProjesi.Models
{
    public class PollutionData
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Enlem alanı zorunludur")]
        [Column(TypeName = "float(53)")]
        public float Latitude { get; set; }
        
        [Required(ErrorMessage = "Boylam alanı zorunludur")]
        [Column(TypeName = "float(53)")]
        public float Longitude { get; set; }
        
        public string? City { get; set; }
        public string? Region { get; set; }
        
        [Required(ErrorMessage = "Metal tipi seçilmelidir")]
        public string MetalType { get; set; }
        
        public DateTime DataRecorded { get; set; }
        
        [Required(ErrorMessage = "Metal değeri girilmelidir")]
        [Column(TypeName = "float(53)")]
        public float Value { get; set; }
        
        [ForeignKey("EnteredByUser")]
        public string EnteredById { get; set; }
        
        public virtual IdentityUser EnteredByUser { get; set; }
    }
}