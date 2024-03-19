namespace OnlineStoreApi.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }   
        public string ProductImagePath { get; set; }
        public float Price { get; set; }
        public int MinimumQuantity { get; set; } = 1;
        public float DiscountRate { get; set; }
    }
}
