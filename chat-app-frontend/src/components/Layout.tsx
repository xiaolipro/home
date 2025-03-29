import React, { useState, useEffect } from 'react';
import { Layout as AntLayout, Menu, Badge } from 'antd';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { MessageOutlined, UserOutlined } from '@ant-design/icons';
import { AuthService } from '../services/authService';
import { PrivateMessage } from './PrivateMessage';
import { UserProfile } from './UserProfile';
import { User } from '../types/user';

const { Header, Content } = AntLayout;

export const Layout: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [messageVisible, setMessageVisible] = useState(false);
    const [user, setUser] = useState<User | null>(null);

    useEffect(() => {
        loadUserInfo();
    }, []);

    const loadUserInfo = async () => {
        try {
            const userInfo = await AuthService.getCurrentUser();
            setUser(userInfo);
        } catch (error) {
            console.error('Failed to load user info:', error);
        }
    };

    const handleLogout = () => {
        AuthService.logout();
        navigate('/login');
    };

    const menuItems = [
        {
            key: '/chat',
            icon: <MessageOutlined />,
            label: '聊天'
        },
        {
            key: '/friends',
            icon: <UserOutlined />,
            label: '好友'
        }
    ];

    return (
        <AntLayout style={{ minHeight: '100vh' }}>
            <Header style={{ padding: 0, background: '#fff' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', height: '100%', padding: '0 24px' }}>
                    <Menu
                        mode="horizontal"
                        selectedKeys={[location.pathname]}
                        items={menuItems}
                        onClick={({ key }) => navigate(key)}
                        style={{ flex: 1 }}
                    />
                    <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
                        <Badge count={5}>
                            <MessageOutlined
                                style={{ fontSize: 20, cursor: 'pointer' }}
                                onClick={() => setMessageVisible(true)}
                            />
                        </Badge>
                        {user && <UserProfile 
                            user={user} 
                            onLogout={handleLogout}
                            onProfileUpdate={setUser}
                        />}
                    </div>
                </div>
            </Header>
            <Content style={{ padding: '24px', background: '#f0f2f5' }}>
                <Outlet />
            </Content>
            <PrivateMessage visible={messageVisible} onClose={() => setMessageVisible(false)} />
        </AntLayout>
    );
}; 