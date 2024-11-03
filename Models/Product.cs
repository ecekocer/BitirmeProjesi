namespace BitirmeProjesi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public required string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public required string ProductDescription { get; set; } 
    }
}

