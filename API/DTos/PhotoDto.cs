namespace API.DTos
{
    public class PhotoDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public bool isApproved { get; set; }
    }
}