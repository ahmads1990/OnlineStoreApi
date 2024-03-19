namespace OnlineStoreApi.Dtos
{
    public class AddNewProductDto
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public IFormFile ProductImage { get; set; }
        public float Price { get; set; }
        public int MinimumQuantity { get; set; } = 1;
        public float DiscountRate { get; set; }
    }
}
