import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Card, message } from 'antd';
import { UserOutlined, LockOutlined, MailOutlined } from '@ant-design/icons';
import { useNavigate, Link } from 'react-router-dom';
import { AuthService } from '../services/authService';
import { Captcha } from '../components/Captcha';

/**
 * 注册页面组件
 */
export const Register: React.FC = () => {
  const navigate = useNavigate();
  const [form] = Form.useForm();
  const [captchaId, setCaptchaId] = useState<string>('');
  const [imageUrl, setImageUrl] = useState<string>('');
  const [loading, setLoading] = useState(false);
  const [captchaLoading, setCaptchaLoading] = useState(true);

  const loadCaptcha = async () => {
    try {
      setCaptchaLoading(true);
      const response = await AuthService.getCaptcha();
      setImageUrl(`data:image/png;base64,${response.imageBase64}`);
      setCaptchaId(response.captchaId);
    } catch (error) {
      console.error('Failed to load captcha:', error);
    } finally {
      setCaptchaLoading(false);
    }
  };

  useEffect(() => {
    loadCaptcha();
  }, []);

  const onFinish = async (values: { username: string; password: string; confirmPassword: string; email: string; captchaCode: string }) => {
    if (!captchaId) {
      message.error('请等待验证码加载完成');
      return;
    }

    try {
      setLoading(true);
      await AuthService.register({
        ...values,
        captchaCode: values.captchaCode,
        captchaId: captchaId
      });
      navigate('/chat');
    } catch (error) {
      loadCaptcha();
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ 
      height: '100vh', 
      display: 'flex', 
      justifyContent: 'center', 
      alignItems: 'center',
      background: 'linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%)'
    }}>
      <Card style={{ width: 400, borderRadius: 8, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
        <div style={{ textAlign: 'center', marginBottom: 24 }}>
          <h1 style={{ fontSize: 24, marginBottom: 8 }}>创建账号</h1>
          <p style={{ color: '#666' }}>请填写以下信息完成注册</p>
        </div>
        <Form
          form={form}
          name="register"
          onFinish={onFinish}
          autoComplete="off"
        >
          <Form.Item
            name="username"
            rules={[{ required: true, message: '请输入用户名' }]}
          >
            <Input 
              prefix={<UserOutlined style={{ color: '#bfbfbf' }} />} 
              placeholder="用户名"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="email"
            rules={[
              { required: true, message: '请输入邮箱' },
              { type: 'email', message: '请输入有效的邮箱地址' }
            ]}
          >
            <Input 
              prefix={<MailOutlined style={{ color: '#bfbfbf' }} />} 
              placeholder="邮箱"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[
              { required: true, message: '请输入密码' },
              { min: 6, message: '密码长度不能小于6位' }
            ]}
          >
            <Input.Password
              prefix={<LockOutlined style={{ color: '#bfbfbf' }} />}
              placeholder="密码"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="confirmPassword"
            dependencies={['password']}
            rules={[
              { required: true, message: '请确认密码' },
              ({ getFieldValue }) => ({
                validator(_, value) {
                  if (!value || getFieldValue('password') === value) {
                    return Promise.resolve();
                  }
                  return Promise.reject(new Error('两次输入的密码不一致'));
                },
              }),
            ]}
          >
            <Input.Password
              prefix={<LockOutlined style={{ color: '#bfbfbf' }} />}
              placeholder="确认密码"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="captchaCode"
            rules={[
              { required: true, message: '请输入验证码' },
              { 
                validator: (_, value) => {
                  if (captchaLoading) {
                    return Promise.reject('请等待验证码加载完成');
                  }
                  return Promise.resolve();
                }
              }
            ]}
          >
            <div style={{ display: 'flex', gap: 8 }}>
              <Input
                placeholder="验证码"
                size="large"
                disabled={captchaLoading}
              />
              <Captcha
                imageUrl={imageUrl}
                onRefresh={loadCaptcha}
              />
            </div>
          </Form.Item>

          <Form.Item>
            <Button 
              type="primary" 
              htmlType="submit" 
              block 
              size="large"
              loading={loading}
              disabled={captchaLoading}
            >
              注册
            </Button>
          </Form.Item>

          <div style={{ textAlign: 'center' }}>
            已有账号？ <Link to="/login">立即登录</Link>
          </div>
        </Form>
      </Card>
    </div>
  );
}; 