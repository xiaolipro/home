import React, { useState } from 'react';
import { Form, Input, Button, Card, message } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import authService from '../services/authService';
import { LoginDto } from '../types/auth';
import Captcha from '../components/Captcha';

/**
 * 登录页面组件
 */
const Login: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [form] = Form.useForm();
  const [captchaId, setCaptchaId] = useState<string>('');

  // 获取用户尝试访问的页面路径
  const from = (location.state as any)?.from?.pathname || '/';

  const onFinish = async (values: LoginDto) => {
    try {
      const response = await authService.login({
        ...values,
        captchaId,
      });
      if (response.success) {
        message.success('登录成功');
        // 登录成功后重定向到用户尝试访问的页面
        navigate(from, { replace: true });
      } else {
        message.error(response.message);
      }
    } catch (error) {
      message.error('登录失败，请稍后重试');
    }
  };

  return (
    <div style={{ 
      height: '100vh', 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center',
      background: '#f0f2f5'
    }}>
      <Card style={{ width: 400 }}>
        <h1 style={{ textAlign: 'center', marginBottom: 24 }}>登录</h1>
        <Form
          form={form}
          name="login"
          onFinish={onFinish}
          autoComplete="off"
        >
          <Form.Item
            name="email"
            rules={[
              { required: true, message: '请输入邮箱' },
              { type: 'email', message: '请输入有效的邮箱地址' }
            ]}
          >
            <Input 
              prefix={<UserOutlined />} 
              placeholder="邮箱" 
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input.Password
              prefix={<LockOutlined />}
              placeholder="密码"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="captchaCode"
            rules={[{ required: true, message: '请输入验证码' }]}
          >
            <Captcha
              captchaId={captchaId}
              onCaptchaIdChange={setCaptchaId}
            />
          </Form.Item>

          <Form.Item>
            <Button type="primary" htmlType="submit" block size="large">
              登录
            </Button>
          </Form.Item>

          <div style={{ textAlign: 'center' }}>
            还没有账号？ <Link to="/register">立即注册</Link>
          </div>
        </Form>
      </Card>
    </div>
  );
};

export default Login; 