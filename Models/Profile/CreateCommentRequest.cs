namespace poplensWebUIGateway.Models.Profile {
    public class CreateCommentRequest {
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
