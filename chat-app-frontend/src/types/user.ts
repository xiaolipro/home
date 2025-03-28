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
    password: string;
    confirmPassword: string;
    email: string;
    captchaCode: string;
    captchaId: string;
}

export interface AuthResponse {
    token: string;
    user: UserDto;
} 