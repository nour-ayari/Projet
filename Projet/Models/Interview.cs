using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Projet.Models
{
    public class Interview
    {
        public int InterviewId { get; set; }

        public int JobApplicationId { get; set; }
        [ForeignKey("JobApplicationId")]
        public JobApplication? JobApplication { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Feedback { get; set; }

        //accepted or cancelled or pending

        public string Status { get; set; }
    }
}
