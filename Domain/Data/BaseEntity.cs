namespace CryptoDataCollector.Data
{
    public class BaseEntity
    {
        public DateTime? CreatedTime { get; set; }
        public string? CreatedName { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string? ModifiedName { get; set; }
    }
}
