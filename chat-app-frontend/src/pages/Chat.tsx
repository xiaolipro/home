import React from 'react';
import { Card, Typography } from 'antd';

const { Title } = Typography;

export const Chat: React.FC = () => {
    return (
        <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
            <Card>
                <Title level={2}>聊天</Title>
                <p>聊天功能开发中...</p>
            </Card>
        </div>
    );
}; 