import React, { useEffect, useState } from 'react';
import { List, Avatar, Button, Badge, message, Modal } from 'antd';
import { UserOutlined, CheckOutlined, CloseOutlined, DeleteOutlined } from '@ant-design/icons';
import { FriendshipDto, FriendshipStatus } from '../types/friendship';
import { FriendshipService } from '../services/friendshipService';
import { formatDistanceToNow } from 'date-fns';
import { zhCN } from 'date-fns/locale';

export const FriendshipList: React.FC = () => {
    const [friends, setFriends] = useState<FriendshipDto[]>([]);
    const [requests, setRequests] = useState<FriendshipDto[]>([]);
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
            message.error('加载好友列表失败');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    const handleAcceptRequest = async (requestId: string) => {
        try {
            await FriendshipService.acceptFriendRequest(requestId);
            message.success('已接受好友请求');
            loadData();
        } catch (error) {
            message.error('接受好友请求失败');
        }
    };

    const handleRejectRequest = async (requestId: string) => {
        try {
            await FriendshipService.rejectFriendRequest(requestId);
            message.success('已拒绝好友请求');
            loadData();
        } catch (error) {
            message.error('拒绝好友请求失败');
        }
    };

    const handleDeleteFriendship = async (friendshipId: string) => {
        Modal.confirm({
            title: '确认删除',
            content: '确定要删除这个好友吗？',
            onOk: async () => {
                try {
                    await FriendshipService.deleteFriendship(friendshipId);
                    message.success('已删除好友');
                    loadData();
                } catch (error) {
                    message.error('删除好友失败');
                }
            }
        });
    };

    const renderFriendItem = (friendship: FriendshipDto) => {
        const friend = friendship.userId === localStorage.getItem('userId') ? friendship.friend : friendship.user;
        return (
            <List.Item
                actions={[
                    <Button
                        type="text"
                        danger
                        icon={<DeleteOutlined />}
                        onClick={() => handleDeleteFriendship(friendship.id)}
                    />
                ]}
            >
                <List.Item.Meta
                    avatar={<Avatar src={friend.avatar} icon={<UserOutlined />} />}
                    title={friend.username}
                    description={`添加于 ${formatDistanceToNow(new Date(friendship.createdAt), { addSuffix: true, locale: zhCN })}`}
                />
            </List.Item>
        );
    };

    const renderRequestItem = (request: FriendshipDto) => (
        <List.Item
            actions={[
                <Button
                    type="text"
                    icon={<CheckOutlined />}
                    onClick={() => handleAcceptRequest(request.id)}
                />,
                <Button
                    type="text"
                    danger
                    icon={<CloseOutlined />}
                    onClick={() => handleRejectRequest(request.id)}
                />
            ]}
        >
            <List.Item.Meta
                avatar={<Avatar src={request.user.avatar} icon={<UserOutlined />} />}
                title={request.user.username}
                description={`请求添加于 ${formatDistanceToNow(new Date(request.createdAt), { addSuffix: true, locale: zhCN })}`}
            />
        </List.Item>
    );

    return (
        <div>
            {requests.length > 0 && (
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
            )}
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