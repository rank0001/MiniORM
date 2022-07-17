using MiniORM.Entities;
using MiniORM.DataAccessLayer;

class Program
{
    static void Main(string[] args)
    {
        Session session = new Session
        {
            Id = 1000,
            DurationInHour = 3,
            LearningObjective = "to understand C# reflection basic!",
            TopicId = 2000
        };
        Session session2 = new Session
        {
            Id = 3000,
            DurationInHour = 12,
            LearningObjective = "to understand C# reflection advanced!",
            TopicId = 2000
        };
        Session session3 = new Session
        {
            Id = 4000,
            DurationInHour = 8,
            LearningObjective = "to comprehend C# generics basic!",
            TopicId = 3000
        };
        Topic topic2 = new Topic
        {
            Id = 3000,
            Title = "C# generics basic ",
            Description = "easy concept",
            CourseId = 1000,
            Sessions = new List<Session>() { (Session)session3 }
        };

        Topic topic = new Topic
        {
            Id = 2000,
            Title = "C# reflection",
            Description = "It's an advanced topic!",
            CourseId = 1000,
            Sessions = new List<Session>() { (Session)session, session2 }
        };
        Phone phone = new Phone()
        {
            Id = 2000,
            Number = "01719369158",
            Extension = "7",
            CountryCode = "+880",
            InstructorId = 3000
        };
        Phone phone2 = new Phone()
        {
            Id = 1000,
            Number = "01701062147",
            Extension = "7",
            CountryCode = "+880",
            InstructorId = 3000
        };
        Address presentAddress = new Address()
        {
            Id = 1000,
            City = "Dhaka",
            Country = "Bangladesh",
            Street = "sector-13",
            InstructorId = 3000
        };
        Address permanentAddress = new Address()
        {
            Id = 2000,
            City = "Kishoregonj",
            Country = "Bangladesh",
            Street = "Jamalpur",
            InstructorId = 3000
        };
        AdmissionTest admissionTest = new AdmissionTest()
        {
            Id = 2000,
            StartDate = new DateTime(2021, 12, 1),
            EndDate = new DateTime(2022, 1, 1),
            TestFees = 200.55M,
            CourseId = 1000,
        };
        AdmissionTest admissionTest2 = new AdmissionTest()
        {
            Id = 3000,
            StartDate = new DateTime(2021, 5, 1),
            EndDate = new DateTime(2022, 1, 1),
            TestFees = 400.55M,
            CourseId = 1000,
        };
        Instructor instructor = new Instructor()
        {
            Id = 3000,
            Name = "Zubayer",
            Email = "zubayerahmed232@gmail.com",
            PermanentAddress = (Address)permanentAddress,
            PresentAddress = (Address)presentAddress,
            PhoneNumbers = new List<Phone>() { (Phone)phone, phone2 },
            CourseId = 1000,
        };
        Course course = new Course()
        {
            Id = 1000,
            Title = " React course",
            Teacher = (Instructor)instructor,
            Topics = new List<Topic>() { (Topic)topic, topic2 },
            Fees = 10000,
            Tests = new List<AdmissionTest>() { (AdmissionTest)admissionTest, admissionTest2 }
        };

        #region insert,delete,update 
        ISqlDataAccess<IId> sql1 = new SqlDataAccess<IId>();
        // sql1.Insert(course);
        // sql1.Delete(course);
        // sql1.Update(course);
        #endregion

        #region deleteById,getById,getAll
        ISqlDataAccess<Course> sql = new SqlDataAccess<Course>();

        //sql.Delete(course.Id);

        //Console.WriteLine(sql.GetById(course.Id));
        //Console.WriteLine(sql.GetAll());
        #endregion

    }
}