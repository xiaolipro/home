import React, { useState, useEffect } from 'react';
import { Input, Space, Image, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import authService from '../services/authService';

interface CaptchaProps {
  value?: string;
  onChange?: (value: string) => void;
  captchaId?: string;
  onCaptchaIdChange?: (id: string) => void;
}

const Captcha: React.FC<CaptchaProps> = ({
  value,
  onChange,
  captchaId,
  onCaptchaIdChange,
}) => {
  const [imageBase64, setImageBase64] = useState<string>('');

  const refreshCaptcha = async () => {
    try {
      const { imageBase64, captchaId } = await authService.getCaptcha();
      setImageBase64(`data:image/png;base64,${imageBase64}`);
      onCaptchaIdChange?.(captchaId);
    } catch (error) {
      console.error('获取验证码失败:', error);
    }
  };

  useEffect(() => {
    refreshCaptcha();
  }, []);

  return (
    <Space>
      <Input
        value={value}
        onChange={(e) => onChange?.(e.target.value)}
        placeholder="请输入验证码"
        maxLength={4}
        style={{ width: 120 }}
      />
      <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
        <Image
          src={imageBase64}
          alt="验证码"
          style={{ height: 40 }}
        />
        <Button
          type="text"
          icon={<ReloadOutlined />}
          onClick={refreshCaptcha}
          title="刷新验证码"
        />
      </div>
    </Space>
  );
};

export default Captcha; 