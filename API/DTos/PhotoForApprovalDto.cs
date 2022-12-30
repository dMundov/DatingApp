namespace API.DTos
{
    public class PhotoForApprovalDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public bool IsApproved { get; set; }
    }
}