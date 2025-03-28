import React from 'react';
import { Card, Typography } from 'antd';
import { FriendshipList } from '../components/FriendshipList';
import { SendFriendRequest } from '../components/SendFriendRequest';

const { Title } = Typography;

export const Friends: React.FC = () => {
    return (
        <div style={{ maxWidth: 800, margin: '0 auto', padding: 24 }}>
            <Card>
                <Title level={2}>好友管理</Title>
                <div style={{ marginBottom: 24 }}>
                    <Title level={4}>添加好友</Title>
                    <SendFriendRequest />
                </div>
                <div>
                    <Title level={4}>好友列表</Title>
                    <FriendshipList />
                </div>
            </Card>
        </div>
    );
}; 