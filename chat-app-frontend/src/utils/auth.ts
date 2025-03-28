/**
 * 检查用户是否已认证
 */
export const isAuthenticated = (): boolean => {
  return !!localStorage.getItem('token');
}; 