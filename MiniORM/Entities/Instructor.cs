namespace MiniORM.Entities
{
    public class Instructor:IId
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Address? PresentAddress { get; set; }
        public Address? PermanentAddress { get; set; }
        public List<Phone>? PhoneNumbers { get; set; }
        public int CourseId { get; set; }
    }
}