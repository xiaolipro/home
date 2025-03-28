import React from 'react';
import { Image, Button } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';

interface CaptchaProps {
    imageUrl: string;
    onRefresh: () => void;
}

export const Captcha: React.FC<CaptchaProps> = ({ imageUrl, onRefresh }) => {
    return (
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
            <Image
                src={imageUrl}
                alt="验证码"
                style={{ height: 40, cursor: 'not-allowed' }}
            />
            <Button
                type="text"
                icon={<ReloadOutlined />}
                onClick={onRefresh}
            />
        </div>
    );
}; 