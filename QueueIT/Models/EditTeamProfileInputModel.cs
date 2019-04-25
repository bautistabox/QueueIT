namespace QueueIT.Models
{
    public class EditTeamProfileInputModel
    {
        public int TeamId { get; set; }
        public string NewTeamName { get; set; }
        public string NewTeamDescription { get; set; }
    }
}