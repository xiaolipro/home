export interface UserDto {
    id: string;
    username: string;
    avatar?: string;
    email: string;
    createdAt: string;
    updatedAt: string;
}

export interface LoginRequest {
    email: string;
    password: string;
    captchaCode: string;
    captchaId: string;
}

export interface RegisterRequest {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    captchaCode: string;
    captchaId: string;
}

export interface AuthResponse {
    token: string;
    user: User;
}

/**
 * 用户信息
 */
export interface User {
    id: string;
    username: string;
    email: string;
    avatar?: string;
    lastLoginAt?: string;
    lastActiveAt?: string;
    createdAt: string;
    updatedAt?: string;
}

/**
 * 发送好友请求
 */
export interface SendFriendRequestRequest {
    userId: string;
}

/**
 * 好友请求
 */
export interface FriendRequest {
    id: string;
    sender: User;
    receiver: User;
    status: 'Pending' | 'Accepted' | 'Rejected';
    createdAt: string;
}

/**
 * 好友关系
 */
export interface Friendship {
    id: string;
    userId: string;
    friendId: string;
    status: FriendshipStatus;
    createdAt: string;
    updatedAt?: string;
    user: User;
    friend: User;
}

export enum FriendshipStatus {
    Pending = 'Pending',
    Accepted = 'Accepted',
    Rejected = 'Rejected'
}

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

/**
 * 更新用户个人信息请求
 */
export interface UpdateProfileRequest {
    username: string;
}

export interface CaptchaResponse {
    imageBase64: string;
    captchaId: string;
} 