using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM1670_Job.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string EmployerId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Age { get; set; }
        public string Phone { get; set; }


        [ForeignKey("EmployerId")]
        public virtual IdentityUser? User_Id { get; set; }

        //1 employer co nhieu job
        public virtual ICollection<Job>? jobs { get; set; }
    }
}
