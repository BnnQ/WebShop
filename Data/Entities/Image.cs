namespace Homework.Data.Entities
{
    public abstract class Image
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = null!;
    }
}