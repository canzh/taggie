namespace Content.Api.EFModels.dto
{
    public class TeamAssignment
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int AssignedItemsCount { get; set; }
    }
}