
CREATE INDEX Idx_userConversationsJoinDateTime
	ON userConversations(joinDateTime)
	
go

CREATE INDEX Idx_userConversationsLeftDateTime
	ON userConversations(leftDateTime)