namespace MiniORM.Entities
{
    public class Phone:IId
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Extension { get; set; }
        public string? CountryCode { get; set; }
        public int InstructorId { get; set; }

    }
}