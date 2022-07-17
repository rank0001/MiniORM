namespace MiniORM.Entities
{
    public class AdmissionTest:IId
    { 
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TestFees { get; set; }
        public int CourseId { get; set; }
    }
}