namespace MiniORM.Entities
{
    public class Session:IId
    {
        public int Id { get; set; }
        public int DurationInHour { get; set; }
        public string? LearningObjective { get; set; }
        public int TopicId { get; set; }

    }
}