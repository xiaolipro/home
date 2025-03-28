import React, { useEffect, useState, useRef } from 'react';
import { Drawer, List, Input, Button, Avatar, Typography } from 'antd';
import { SendOutlined, UserOutlined } from '@ant-design/icons';
import { PrivateMessage as PrivateMessageType, ChatSession, SendMessageRequest } from '../types/message';
import { MessageService } from '../services/messageService';
import { formatDistanceToNow } from 'date-fns';
import { zhCN } from 'date-fns/locale';

const { Text } = Typography;

interface PrivateMessageProps {
    visible: boolean;
    onClose: () => void;
}

export const PrivateMessage: React.FC<PrivateMessageProps> = ({ visible, onClose }) => {
    const [sessions, setSessions] = useState<ChatSession[]>([]);
    const [selectedUser, setSelectedUser] = useState<ChatSession | null>(null);
    const [messages, setMessages] = useState<PrivateMessageType[]>([]);
    const [inputValue, setInputValue] = useState('');
    const [unreadCount, setUnreadCount] = useState(0);
    const messagesEndRef = useRef<HTMLDivElement>(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        if (visible) {
            loadSessions();
            loadUnreadCount();
        }
    }, [visible]);

    useEffect(() => {
        if (selectedUser) {
            loadMessages(selectedUser.userId);
        }
    }, [selectedUser]);

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const loadSessions = async () => {
        try {
            const data = await MessageService.getChatSessions();
            setSessions(data);
        } catch (error) {
            console.error('加载会话列表失败:', error);
        }
    };

    const loadUnreadCount = async () => {
        try {
            const count = await MessageService.getUnreadCount();
            setUnreadCount(count);
        } catch (error) {
            console.error('加载未读消息数失败:', error);
        }
    };

    const loadMessages = async (userId: string) => {
        try {
            const data = await MessageService.getMessages(userId);
            setMessages(data);
        } catch (error) {
            console.error('加载消息记录失败:', error);
        }
    };

    const handleSendMessage = async () => {
        if (!selectedUser || !inputValue.trim()) return;

        try {
            const request: SendMessageRequest = {
                receiverId: selectedUser.userId,
                content: inputValue.trim()
            };

            await MessageService.sendMessage(request);
            setInputValue('');
            await loadMessages(selectedUser.userId);
            await loadSessions();
            await loadUnreadCount();
        } catch (error) {
            console.error('发送消息失败:', error);
        }
    };

    const handleUserSelect = (session: ChatSession) => {
        setSelectedUser(session);
    };

    return (
        <Drawer
            title="私信"
            placement="right"
            onClose={onClose}
            open={visible}
            width={400}
        >
            <div style={{ display: 'flex', height: '100%' }}>
                <div style={{ width: 120, borderRight: '1px solid #f0f0f0', overflowY: 'auto' }}>
                    <List
                        dataSource={sessions}
                        renderItem={(session) => (
                            <List.Item
                                style={{
                                    cursor: 'pointer',
                                    padding: '8px',
                                    backgroundColor: selectedUser?.id === session.id ? '#f0f0f0' : 'transparent'
                                }}
                                onClick={() => handleUserSelect(session)}
                            >
                                <List.Item.Meta
                                    avatar={<Avatar src={session.avatar} icon={<UserOutlined />} />}
                                    title={<Text ellipsis>{session.username}</Text>}
                                    description={
                                        <Text type="secondary" style={{ fontSize: 12 }}>
                                            {formatDistanceToNow(new Date(session.lastMessageTime), { addSuffix: true, locale: zhCN })}
                                        </Text>
                                    }
                                />
                            </List.Item>
                        )}
                    />
                </div>
                <div style={{ flex: 1, display: 'flex', flexDirection: 'column' }}>
                    {selectedUser ? (
                        <>
                            <div style={{ flex: 1, overflowY: 'auto', padding: '16px' }}>
                                <List
                                    dataSource={messages}
                                    renderItem={(message) => {
                                        const isMe = message.senderId === localStorage.getItem('userId');
                                        return (
                                            <div style={{ marginBottom: 16, textAlign: isMe ? 'right' : 'left' }}>
                                                <div style={{ display: 'inline-flex', alignItems: 'flex-start', gap: 8 }}>
                                                    {!isMe && <Avatar src={message.sender.avatar} icon={<UserOutlined />} />}
                                                    <div
                                                        style={{
                                                            maxWidth: '70%',
                                                            padding: '8px 12px',
                                                            borderRadius: '8px',
                                                            backgroundColor: isMe ? '#1890ff' : '#f0f0f0',
                                                            color: isMe ? '#fff' : 'inherit'
                                                        }}
                                                    >
                                                        <Text style={{ color: isMe ? '#fff' : 'inherit' }}>
                                                            {message.content}
                                                        </Text>
                                                        <div style={{ fontSize: 12, marginTop: 4, opacity: 0.7 }}>
                                                            {formatDistanceToNow(new Date(message.createdAt), { addSuffix: true, locale: zhCN })}
                                                        </div>
                                                    </div>
                                                    {isMe && <Avatar src={message.sender.avatar} icon={<UserOutlined />} />}
                                                </div>
                                            </div>
                                        );
                                    }}
                                />
                                <div ref={messagesEndRef} />
                            </div>
                            <div style={{ padding: '16px', borderTop: '1px solid #f0f0f0' }}>
                                <div style={{ display: 'flex', gap: 8 }}>
                                    <Input.TextArea
                                        value={inputValue}
                                        onChange={(e) => setInputValue(e.target.value)}
                                        placeholder="输入消息..."
                                        autoSize={{ minRows: 1, maxRows: 4 }}
                                        onPressEnter={(e) => {
                                            if (!e.shiftKey) {
                                                e.preventDefault();
                                                handleSendMessage();
                                            }
                                        }}
                                    />
                                    <Button
                                        type="primary"
                                        icon={<SendOutlined />}
                                        onClick={handleSendMessage}
                                        disabled={!inputValue.trim()}
                                    />
                                </div>
                            </div>
                        </>
                    ) : (
                        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                            <Text type="secondary">选择一个会话开始聊天</Text>
                        </div>
                    )}
                </div>
            </div>
        </Drawer>
    );
}; 