import React, { useState } from 'react';
import { Input, Button, message } from 'antd';
import { UserAddOutlined } from '@ant-design/icons';
import { FriendshipService } from '../services/friendshipService';

export const SendFriendRequest: React.FC = () => {
    const [friendId, setFriendId] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        if (!friendId.trim()) {
            message.warning('请输入用户ID');
            return;
        }

        try {
            setLoading(true);
            await FriendshipService.sendFriendRequest({ friendId });
            message.success('好友请求已发送');
            setFriendId('');
        } catch (error) {
            message.error('发送好友请求失败');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ display: 'flex', gap: 8 }}>
            <Input
                placeholder="输入用户ID"
                value={friendId}
                onChange={(e) => setFriendId(e.target.value)}
                style={{ width: 200 }}
            />
            <Button
                type="primary"
                icon={<UserAddOutlined />}
                onClick={handleSubmit}
                loading={loading}
            >
                发送好友请求
            </Button>
        </div>
    );
}; 