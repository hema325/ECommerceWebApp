
CREATE INDEX idx_messagesIsRead
	ON messages (isRead)

go

CREATE INDEX idx_messagesSenderId
	ON messages (senderId)

go

CREATE INDEX idx_messagesConversationId
	ON messages (conversationId)
