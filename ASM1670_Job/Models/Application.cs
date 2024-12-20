using System.ComponentModel.DataAnnotations.Schema;

namespace ASM1670_Job.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string ApplicationLatter { get; set; }
        public DateTime ApplicationDate { get; set; }

        public int Seeker_Id { get; set; }

        [ForeignKey("Seeker_Id")]
        public virtual Seeker? Seeker { get; set; }

        public int Job_Id { get; set; }
        [ForeignKey("Job_Id")]
        public virtual Job? Job { get; set; }


        public string? Status { get; set; }
    }
}
