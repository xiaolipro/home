import axios from 'axios';
import { message } from 'antd';
import ErrorService from './ErrorService';

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
        // 记录请求错误
        ErrorService.collectError(error, {
            type: 'request_error',
            config: error.config
        });
        return Promise.reject(error);
    }
);

// 响应拦截器：处理错误
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        // 记录所有API错误
        const apiError = new Error(
            `API Error: ${error.response?.status} - ${error.response?.data?.message || error.message}`
        );
        apiError.stack = `URL: ${error.config?.url}\nMethod: ${error.config?.method}\n${error.stack}`;
        
        // 记录请求参数
        const requestData = {
            url: error.config?.url,
            method: error.config?.method,
            params: error.config?.params,
            data: error.config?.data,
            headers: error.config?.headers,
            response: error.response?.data
        };
        
        ErrorService.collectError(apiError, requestData);

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