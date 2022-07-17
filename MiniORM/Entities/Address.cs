namespace MiniORM.Entities
{
    public class Address:IId
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int InstructorId { get; set; }

    }
}