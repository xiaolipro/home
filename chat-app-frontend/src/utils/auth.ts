import { UserDto } from '../types/auth';

/**
 * 存储用户信息到本地存储
 */
export const setUserToStorage = (user: UserDto): void => {
  localStorage.setItem('user', JSON.stringify(user));
};

/**
 * 从本地存储获取用户信息
 */
export const getUserFromStorage = (): UserDto | null => {
  const userStr = localStorage.getItem('user');
  return userStr ? JSON.parse(userStr) : null;
};

/**
 * 清除本地存储的用户信息
 */
export const clearUserFromStorage = (): void => {
  localStorage.removeItem('user');
  localStorage.removeItem('token');
};

/**
 * 检查用户是否已登录
 */
export const isAuthenticated = (): boolean => {
  return !!localStorage.getItem('token') && !!localStorage.getItem('user');
}; 