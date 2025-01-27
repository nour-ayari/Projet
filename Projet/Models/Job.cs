using Microsoft.AspNetCore.Builder;

namespace Projet.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public string Title { get; set; }
        public int Positions { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<JobApplication> Applications { get; set; }= new List<JobApplication>();
    }
}
