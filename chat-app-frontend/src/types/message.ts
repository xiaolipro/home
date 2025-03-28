/**
 * 私信消息类型
 */
export interface PrivateMessage {
  id: string;
  senderId: string;
  receiverId: string;
  content: string;
  createdAt: string;
  isRead: boolean;
  sender: {
    id: string;
    username: string;
    avatar: string;
  };
  receiver: {
    id: string;
    username: string;
    avatar: string;
  };
}

/**
 * 私信会话类型
 */
export interface ChatSession {
  id: string;
  userId: string;
  username: string;
  avatar: string;
  lastMessage: string;
  lastMessageTime: string;
  unreadCount: number;
}

/**
 * 发送私信请求
 */
export interface SendMessageRequest {
  receiverId: string;
  content: string;
}

/**
 * 私信列表响应
 */
export interface MessageListResponse {
  messages: PrivateMessage[];
}

/**
 * 私信会话列表响应
 */
export interface ChatSessionListResponse {
  sessions: ChatSession[];
} 