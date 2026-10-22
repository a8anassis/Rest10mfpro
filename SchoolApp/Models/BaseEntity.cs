namespace SchoolApp.Models
{
    public abstract class BaseEntity
    {
        public DateTime InsertedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public bool IsDeleted { get; set; } = false;        // Soft delete
        public DateTime DeletedAt { get; set; }             // Soft delete
    }
}
