namespace Projet.Models
{
    public class JobViewModel
    {
        public int JobId { get; set; }
        public Job job { get; set; }
        public string Title { get; set; }
        public string DaysAgo { get; set; }
        public int Positions { get; set; }
        public int ApplicationsCount { get; set; }
        public int InterviewedCount { get; set; }
        public int RejectedCount { get; set; }
        public int FeedbackPendingCount { get; set; }
        public int PositionsLeft { get; set; }
    }
}
