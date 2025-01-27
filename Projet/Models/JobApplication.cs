using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projet.Models
{
    public class JobApplication
    {
        
        public int JobApplicationId { get; set; }

        [Required]
        public int JobId { get; set; }
        public Job Job { get; set; }

        [Required]
        public string ApplicantName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string Resume { get; set; } // Path to the saved file
        public string Feedback { get; set; }
        public string Status { get; set; } = "Pending";//accepted or rejected or pending

        public DateTime AppliedDate { get; set; }
        public ICollection<Interview> Interviews { get; set; }
    }

}
