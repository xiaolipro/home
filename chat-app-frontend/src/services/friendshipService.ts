import { apiClient } from './apiClient';
import { User } from '../types/user';

/**
 * 好友服务类
 */
export class FriendshipService {
    /**
     * 获取好友列表
     */
    static async getFriends(): Promise<User[]> {
        const response = await apiClient.get<User[]>('/api/friendships/friends');
        return response.data;
    }

    /**
     * 获取待处理的好友请求
     */
    static async getFriendRequests(): Promise<User[]> {
        const response = await apiClient.get<User[]>('/api/friendships/requests');
        return response.data;
    }

    /**
     * 搜索用户
     */
    static async searchUsers(keyword: string): Promise<User[]> {
        const response = await apiClient.get<User[]>('/api/friendships/search', {
            params: { keyword }
        });
        return response.data;
    }

    /**
     * 发送好友请求
     */
    static async sendFriendRequest(userId: string): Promise<void> {
        await apiClient.post('/api/friendships/requests', { userId });
    }

    /**
     * 接受好友请求
     */
    static async acceptFriendRequest(userId: string): Promise<void> {
        await apiClient.put(`/api/friendships/requests/${userId}/accept`);
    }

    /**
     * 拒绝好友请求
     */
    static async rejectFriendRequest(userId: string): Promise<void> {
        await apiClient.put(`/api/friendships/requests/${userId}/reject`);
    }

    /**
     * 删除好友关系
     */
    static async deleteFriendship(userId: string): Promise<void> {
        await apiClient.delete(`/api/friendships/${userId}`);
    }
} 