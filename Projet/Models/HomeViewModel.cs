namespace Projet.Models
{
    public class HomeViewModel
    {
        public int OpenPositions { get; set; }
        public int TotalApplications { get; set; }
        public int TodayMeetings { get; set; }
        public int ApplicationsToSchedule { get; set; }
        public int JobAppToEvaluate { get; set; }
    public int InterviewsToEvalute { get; set; }
public IEnumerable<Interview> TodayInterviews { get; set; } = new List<Interview>();
        public IEnumerable<Interview> TomorrowInterviews { get; set; } = new List<Interview>();


    }
}
