import React, { useEffect, useState } from 'react';
import { List, Avatar, Button, Badge, message, Modal } from 'antd';
import { UserOutlined, CheckOutlined, CloseOutlined, DeleteOutlined } from '@ant-design/icons';
import { User, FriendshipStatus } from '../types/user';
import { FriendshipService } from '../services/friendshipService';
import { formatDistanceToNow } from 'date-fns';
import { zhCN } from 'date-fns/locale';

export const FriendshipList: React.FC = () => {
    const [friends, setFriends] = useState<User[]>([]);
    const [requests, setRequests] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);

    const loadData = async () => {
        try {
            setLoading(true);
            const [friendsData, requestsData] = await Promise.all([
                FriendshipService.getFriends(),
                FriendshipService.getFriendRequests()
            ]);
            setFriends(friendsData);
            setRequests(requestsData);
        } catch (error) {
            console.error('Failed to load data:', error);
            message.error('加载数据失败');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleAcceptRequest = async (userId: string) => {
        try {
            await FriendshipService.acceptFriendRequest(userId);
            message.success('已接受好友请求');
            loadData();
        } catch (error) {
            console.error('Failed to accept request:', error);
            message.error('接受好友请求失败');
        }
    };

    const handleRejectRequest = async (userId: string) => {
        try {
            await FriendshipService.rejectFriendRequest(userId);
            message.success('已拒绝好友请求');
            loadData();
        } catch (error) {
            console.error('Failed to reject request:', error);
            message.error('拒绝好友请求失败');
        }
    };

    const handleDeleteFriendship = async (userId: string) => {
        Modal.confirm({
            title: '确认删除',
            content: '确定要删除这个好友吗？',
            onOk: async () => {
                try {
                    await FriendshipService.deleteFriendship(userId);
                    message.success('已删除好友');
                    loadData();
                } catch (error) {
                    console.error('Failed to delete friendship:', error);
                    message.error('删除好友失败');
                }
            }
        });
    };

    const renderFriendItem = (friend: User) => (
        <List.Item
            actions={[
                <Button
                    type="text"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => handleDeleteFriendship(friend.id)}
                />
            ]}
        >
            <List.Item.Meta
                avatar={<Avatar src={friend.avatar} icon={<UserOutlined />} />}
                title={friend.username}
                description={
                    <div>
                        <div>{friend.email}</div>
                        <div style={{ color: '#999', fontSize: '12px' }}>
                            {friend.lastActiveAt && (
                                <span>
                                    最后活跃于 {formatDistanceToNow(new Date(friend.lastActiveAt), { addSuffix: true, locale: zhCN })}
                                </span>
                            )}
                        </div>
                    </div>
                }
            />
        </List.Item>
    );

    const renderRequestItem = (user: User) => (
        <List.Item
            actions={[
                <Button
                    type="text"
                    icon={<CheckOutlined />}
                    onClick={() => handleAcceptRequest(user.id)}
                />,
                <Button
                    type="text"
                    danger
                    icon={<CloseOutlined />}
                    onClick={() => handleRejectRequest(user.id)}
                />
            ]}
        >
            <List.Item.Meta
                avatar={<Avatar src={user.avatar} icon={<UserOutlined />} />}
                title={user.username}
                description={
                    <div>
                        <div>{user.email}</div>
                        <div style={{ color: '#999', fontSize: '12px' }}>
                            {user.lastActiveAt && (
                                <span>
                                    最后活跃于 {formatDistanceToNow(new Date(user.lastActiveAt), { addSuffix: true, locale: zhCN })}
                                </span>
                            )}
                        </div>
                    </div>
                }
            />
        </List.Item>
    );

    return (
        <div>
            <div style={{ marginBottom: 24 }}>
                <Badge count={requests.length}>
                    <h3>好友请求</h3>
                </Badge>
                <List
                    loading={loading}
                    itemLayout="horizontal"
                    dataSource={requests}
                    renderItem={renderRequestItem}
                />
            </div>
            <div>
                <h3>我的好友</h3>
                <List
                    loading={loading}
                    itemLayout="horizontal"
                    dataSource={friends}
                    renderItem={renderFriendItem}
                />
            </div>
        </div>
    );
}; 