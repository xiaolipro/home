import React, { useState } from 'react';
import { Layout as AntLayout, Menu, Badge } from 'antd';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { MessageOutlined, UserOutlined, LogoutOutlined } from '@ant-design/icons';
import { AuthService } from '../services/authService';
import { PrivateMessage } from './PrivateMessage';

const { Header, Content } = AntLayout;

export const Layout: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const [messageVisible, setMessageVisible] = useState(false);

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
                        <Menu
                            mode="horizontal"
                            items={[
                                {
                                    key: 'logout',
                                    icon: <LogoutOutlined />,
                                    label: '退出',
                                    onClick: handleLogout
                                }
                            ]}
                        />
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