import { UserDto } from './user';

export enum FriendshipStatus {
    Pending = 'Pending',
    Accepted = 'Accepted',
    Rejected = 'Rejected'
}

export interface FriendshipDto {
    id: string;
    userId: string;
    friendId: string;
    status: FriendshipStatus;
    createdAt: string;
    updatedAt: string;
    user: UserDto;
    friend: UserDto;
}

export interface FriendshipListResponse {
    items: FriendshipDto[];
    total: number;
} 