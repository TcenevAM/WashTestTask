namespace Data.Contracts
{
    public interface ProductCreated
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}