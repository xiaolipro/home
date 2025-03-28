import { FriendshipDto, SendFriendRequestRequest } from '../types/friendship';
import { apiClient } from './apiClient';

export class FriendshipService {
    static async getFriends(): Promise<FriendshipDto[]> {
        const response = await apiClient.get('/api/friendships');
        return response.data;
    }

    static async getFriendRequests(): Promise<FriendshipDto[]> {
        const response = await apiClient.get('/api/friendships/requests');
        return response.data;
    }

    static async sendFriendRequest(request: SendFriendRequestRequest): Promise<FriendshipDto> {
        const response = await apiClient.post('/api/friendships/requests', request);
        return response.data;
    }

    static async acceptFriendRequest(requestId: string): Promise<void> {
        await apiClient.put(`/api/friendships/requests/${requestId}/accept`);
    }

    static async rejectFriendRequest(requestId: string): Promise<void> {
        await apiClient.put(`/api/friendships/requests/${requestId}/reject`);
    }

    static async deleteFriendship(friendshipId: string): Promise<void> {
        await apiClient.delete(`/api/friendships/${friendshipId}`);
    }
} 