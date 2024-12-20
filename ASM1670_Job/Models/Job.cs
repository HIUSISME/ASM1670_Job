using System.ComponentModel.DataAnnotations.Schema;

namespace ASM1670_Job.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public string Title { get; set; }

        public DateTime DeadLine { get; set; }

        public int Salary { get; set; }

        public int? Employer_id { get; set; }

        [ForeignKey("Employer_id")]
        public virtual Employer? Employer { get; set; }

        public virtual ICollection<Application>? Application { get; set; }
    }
}
