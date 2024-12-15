using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesi.Models.ViewModels
{
    public class PollutionEntryViewModel
    {
        [Required(ErrorMessage = "Enlem alanı zorunludur")]
        public float Latitude { get; set; }
        
        [Required(ErrorMessage = "Boylam alanı zorunludur")]
        public float Longitude { get; set; }
        
        [Required(ErrorMessage = "Metal tipi seçilmelidir")]
        public string MetalType { get; set; }
        
        [Required(ErrorMessage = "Metal değeri girilmelidir")]
        public float Value { get; set; }

        [Required(ErrorMessage = "Yıl alanı zorunludur")]
        [Range(1960, 2025, ErrorMessage = "Yıl 1960 ile günümüz arasında olmalıdır")]
        public int Year { get; set; } = DateTime.Now.Year; // Default değer olarak güncel yıl
    }
} 