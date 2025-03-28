import api from './api';
import { RegisterDto, LoginDto, AuthResponseDto, UserDto } from '../types/auth';
import { setUserToStorage, clearUserFromStorage } from '../utils/auth';

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
class AuthService {
  /**
   * 获取验证码
   * @returns 验证码图片和ID
   */
  async getCaptcha(): Promise<CaptchaResponseDto> {
    const response = await api.get<CaptchaResponseDto>('/auth/captcha');
    return response.data;
  }

  /**
   * 用户注册
   * @param registerDto 注册信息
   * @returns 认证响应
   */
  async register(registerDto: RegisterDto): Promise<AuthResponseDto> {
    const response = await api.post<AuthResponseDto>('/auth/register', registerDto);
    if (response.data.success) {
      localStorage.setItem('token', response.data.token);
      setUserToStorage(response.data.user);
    }
    return response.data;
  }

  /**
   * 用户登录
   * @param loginDto 登录信息
   * @returns 认证响应
   */
  async login(loginDto: LoginDto): Promise<AuthResponseDto> {
    const response = await api.post<AuthResponseDto>('/auth/login', loginDto);
    if (response.data.success) {
      localStorage.setItem('token', response.data.token);
      setUserToStorage(response.data.user);
    }
    return response.data;
  }

  /**
   * 用户登出
   */
  logout(): void {
    clearUserFromStorage();
  }

  /**
   * 获取当前用户信息
   * @returns 当前用户信息或null
   */
  getCurrentUser(): UserDto | null {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
}

export default new AuthService(); 