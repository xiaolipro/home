/**
 * 用户数据传输对象
 */
export interface UserDto {
  id: string;
  username: string;
  email: string;
  avatar: string;
  isOnline: boolean;
  lastSeen: string;
}

/**
 * 注册请求数据传输对象
 */
export interface RegisterDto {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  captchaId: string;
  captchaCode: string;
}

/**
 * 登录请求数据传输对象
 */
export interface LoginDto {
  email: string;
  password: string;
  captchaId: string;
  captchaCode: string;
}

/**
 * 认证响应数据传输对象
 */
export interface AuthResponseDto {
  success: boolean;
  message: string;
  token: string;
  user: UserDto;
} 