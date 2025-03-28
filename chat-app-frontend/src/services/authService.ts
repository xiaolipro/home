import { LoginRequest, RegisterRequest, AuthResponse } from '../types/user';
import { apiClient } from './apiClient';

/**
 * 验证码响应数据传输对象
 */
interface CaptchaResponseDto {
  imageBase64: string;
  captchaId: string;
}

/**
 * 认证服务类
 */
export class AuthService {
  /**
   * 存储认证信息
   */
  private static storeAuthData(response: AuthResponse): void {
    localStorage.setItem('token', response.token);
    localStorage.setItem('userId', response.user.id);
    localStorage.setItem('username', response.user.username);
  }

  /**
   * 清除认证信息
   */
  private static clearAuthData(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('username');
  }

  /**
   * 获取验证码
   * @returns 验证码图片和ID
   */
  static async getCaptcha(): Promise<CaptchaResponseDto> {
    try {
      const response = await apiClient.get<CaptchaResponseDto>('/api/auth/captcha');
      return response.data;
    } catch (error) {
      console.error('获取验证码失败:', error);
      throw error;
    }
  }

  /**
   * 用户注册
   * @param request 注册信息
   * @returns 认证响应
   */
  static async register(request: RegisterRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/api/auth/register', request);
      this.storeAuthData(response.data);
      return response.data;
    } catch (error) {
      console.error('注册失败:', error);
      throw error;
    }
  }

  /**
   * 用户登录
   * @param request 登录信息
   * @returns 认证响应
   */
  static async login(request: LoginRequest): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>('/api/auth/login', request);
      this.storeAuthData(response.data);
      return response.data;
    } catch (error) {
      console.error('登录失败:', error);
      throw error;
    }
  }

  /**
   * 用户登出
   */
  static logout(): void {
    this.clearAuthData();
  }

  /**
   * 获取当前用户信息
   * @returns 当前用户信息或null
   */
  static getCurrentUser(): { id: string; username: string } | null {
    const userId = localStorage.getItem('userId');
    const username = localStorage.getItem('username');
    return userId && username ? { id: userId, username } : null;
  }
} 