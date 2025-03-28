import React, { useEffect, useState, useRef } from 'react';
import { Drawer, List, Input, Button, Avatar, Typography } from 'antd';
import { SendOutlined } from '@ant-design/icons';
import { PrivateMessage as PrivateMessageType, ChatSession, SendMessageRequest } from '../types/message';
import { PrivateMessageService } from '../services/messageService';
import { formatDistanceToNow } from 'date-fns';
import { zhCN } from 'date-fns/locale';

const { Text } = Typography;

interface PrivateMessageProps {
    visible: boolean;
    onClose: () => void;
}

export const PrivateMessage: React.FC<PrivateMessageProps> = ({ visible, onClose }) => {
    const [sessions, setSessions] = useState<ChatSession[]>([]);
    const [selectedUser, setSelectedUser] = useState<string | null>(null);
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
            loadMessages(selectedUser);
        }
    }, [selectedUser]);

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    const loadSessions = async () => {
        try {
            const data = await PrivateMessageService.getChatSessions();
            setSessions(data);
        } catch (error) {
            console.error('Failed to load chat sessions:', error);
        }
    };

    const loadUnreadCount = async () => {
        try {
            const count = await PrivateMessageService.getUnreadCount();
            setUnreadCount(count);
        } catch (error) {
            console.error('Failed to load unread count:', error);
        }
    };

    const loadMessages = async (userId: string) => {
        try {
            const data = await PrivateMessageService.getMessages(userId);
            setMessages(data);
        } catch (error) {
            console.error('Failed to load messages:', error);
        }
    };

    const handleSendMessage = async () => {
        if (!inputValue.trim() || !selectedUser) return;

        try {
            const request: SendMessageRequest = {
                receiverId: selectedUser,
                content: inputValue.trim()
            };

            await PrivateMessageService.sendMessage(request);
            setInputValue('');
            loadMessages(selectedUser);
            loadSessions();
            loadUnreadCount();
        } catch (error) {
            console.error('Failed to send message:', error);
        }
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
                <div style={{ width: 200, borderRight: '1px solid #f0f0f0', overflowY: 'auto' }}>
                    <List
                        dataSource={sessions}
                        renderItem={(session) => (
                            <List.Item
                                style={{
                                    cursor: 'pointer',
                                    padding: '8px',
                                    backgroundColor: selectedUser === session.userId ? '#f0f0f0' : 'transparent'
                                }}
                                onClick={() => setSelectedUser(session.userId)}
                            >
                                <List.Item.Meta
                                    avatar={<Avatar src={session.avatar} />}
                                    title={session.username}
                                    description={
                                        <Text ellipsis style={{ maxWidth: 150 }}>
                                            {session.lastMessage}
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
                                {messages.map((message) => (
                                    <div
                                        key={message.id}
                                        style={{
                                            marginBottom: '8px',
                                            display: 'flex',
                                            justifyContent: message.senderId === localStorage.getItem('userId') ? 'flex-end' : 'flex-start'
                                        }}
                                    >
                                        <div
                                            style={{
                                                maxWidth: '70%',
                                                padding: '8px 12px',
                                                borderRadius: '8px',
                                                backgroundColor: message.senderId === localStorage.getItem('userId') ? '#1890ff' : '#f0f0f0',
                                                color: message.senderId === localStorage.getItem('userId') ? 'white' : 'black'
                                            }}
                                        >
                                            {message.content}
                                        </div>
                                    </div>
                                ))}
                                <div ref={messagesEndRef} />
                            </div>
                            <div style={{ padding: '16px', borderTop: '1px solid #f0f0f0' }}>
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
                                    style={{ marginTop: '8px', float: 'right' }}
                                >
                                    发送
                                </Button>
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