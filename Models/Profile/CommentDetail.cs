﻿namespace poplensWebUIGateway.Models.Profile {
    public class CommentDetail : Comment {
        public string Username {get;set; }
        public List<CommentDetail> DetailedReplies { get; set; }
    }
}
