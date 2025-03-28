import React, { useEffect, useState } from 'react';
import { Form, Input, Button, Card, message } from 'antd';
import { MailOutlined, LockOutlined } from '@ant-design/icons';
import { useNavigate, Link } from 'react-router-dom';
import { AuthService } from '../services/authService';
import { Captcha } from '../components/Captcha';

/**
 * 登录页面组件
 */
export const Login: React.FC = () => {
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
      message.error('加载验证码失败');
    } finally {
      setCaptchaLoading(false);
    }
  };

  useEffect(() => {
    loadCaptcha();
  }, []);

  const onFinish = async (values: { email: string; password: string; captchaCode: string }) => {
    if (!captchaId) {
      message.error('请等待验证码加载完成');
      return;
    }

    try {
      setLoading(true);
      await AuthService.login({
        email: values.email,
        password: values.password,
        captchaCode: values.captchaCode,
        captchaId: captchaId
      });
      message.success('登录成功');
      navigate('/chat');
    } catch (error: any) {
      message.error(error.response?.data?.message || '登录失败，请重试');
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
          <h1 style={{ fontSize: 24, marginBottom: 8 }}>欢迎回来</h1>
          <p style={{ color: '#666' }}>请登录您的账号</p>
        </div>
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
              prefix={<MailOutlined style={{ color: '#bfbfbf' }} />} 
              placeholder="邮箱"
              size="large"
            />
          </Form.Item>

          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input.Password
              prefix={<LockOutlined style={{ color: '#bfbfbf' }} />}
              placeholder="密码"
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