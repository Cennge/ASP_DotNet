namespace asp_all.Data.Entities
{
    public record ShopSection
    {
        public Guid Id { get; set; }

        public Guid? ParentId { get; set; }

        public String Title { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String Slug { get; set; } = null!;

        public String ImageUrl { get; set; } = null!;

        public DateTime? DeletedAt{ get; set; }

        public int OrderInPrice { get; set; } = 10000;

        public ICollection<ShopProduct> Products { get; set; } = [];
    }
}
