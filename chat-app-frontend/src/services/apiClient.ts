import axios from 'axios';
import { message } from 'antd';

// 创建 axios 实例
export const apiClient = axios.create({
    baseURL: 'http://localhost:5000', // ASP.NET Core 默认端口
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json'
    }
});

// 请求拦截器：添加 token
apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

// 响应拦截器：处理错误
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            // 未授权，清除 token 并跳转到登录页
            localStorage.removeItem('token');
            localStorage.removeItem('userId');
            localStorage.removeItem('username');
            window.location.href = '/login';
        } else if (error.response?.status === 400) {
            // 处理业务错误
            const errorMessage = error.response.data?.message || '操作失败';
            message.error(errorMessage);
        }
        return Promise.reject(error);
    }
); 