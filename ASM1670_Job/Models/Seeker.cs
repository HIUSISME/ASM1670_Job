using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM1670_Job.Models
{
    public class Seeker
    {
        public int Id { get; set; }
        public string SeekerId { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public string? Picture { get; set; }

        [NotMapped]

        public IFormFile? PictureImage { get; set; }

        [ForeignKey("SeekerId")]
        public virtual IdentityUser? User_ID { get; set; }

        //1 seeker co nhieu application
        public virtual ICollection<Application>? Application { get; set; }
    }
}
