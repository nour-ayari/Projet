namespace Projet.Models
{
    public class HomeViewModel
    {
        public int OpenPositions { get; set; }
        public int TotalApplications { get; set; }
        public int TodayMeetings { get; set; }
        public int ApplicationsToSchedule { get; set; }
        public IEnumerable<Interview> TodayInterviews { get; set; } = new List<Interview>();

    }
}
