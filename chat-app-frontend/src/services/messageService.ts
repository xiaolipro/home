import { PrivateMessage, ChatSession, SendMessageRequest } from '../types/message';
import { apiClient } from './apiClient';

/**
 * 私信服务类
 */
export class PrivateMessageService {
  /**
   * 获取私信会话列表
   */
  static async getChatSessions(): Promise<ChatSession[]> {
    const response = await apiClient.get<ChatSession[]>('/api/privatemessages/sessions');
    return response.data;
  }

  /**
   * 获取与指定用户的私信记录
   * @param userId 用户ID
   */
  static async getMessages(userId: string): Promise<PrivateMessage[]> {
    const response = await apiClient.get<PrivateMessage[]>(`/api/privatemessages/${userId}`);
    return response.data;
  }

  /**
   * 发送私信
   * @param request 发送私信请求
   */
  static async sendMessage(request: SendMessageRequest): Promise<PrivateMessage> {
    const response = await apiClient.post<PrivateMessage>('/api/privatemessages', request);
    return response.data;
  }

  /**
   * 标记私信为已读
   * @param messageId 私信ID
   */
  static async markAsRead(messageId: string): Promise<void> {
    await apiClient.put(`/api/privatemessages/${messageId}/read`);
  }

  /**
   * 获取未读私信数量
   */
  static async getUnreadCount(): Promise<number> {
    const response = await apiClient.get<{ count: number }>('/api/privatemessages/unread/count');
    return response.data.count;
  }
} 