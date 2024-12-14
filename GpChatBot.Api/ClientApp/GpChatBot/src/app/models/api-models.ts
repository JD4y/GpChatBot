export interface UpdateBotMessageStatusRequest {
  chatId: string;
  messageId: string;
  like: boolean;
}

export interface ChatApiModel {
  id: string;
  startedAt: Date;
  messages: MessageApiModel[];
}

export interface MessageApiModel {
  id: string;
  chatId: string;
  message: string;
  createdAt: Date;
  isLiked: boolean | null;
  isBot: boolean;
}
