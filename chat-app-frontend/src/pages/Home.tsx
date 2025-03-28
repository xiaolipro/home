import React, { useState } from 'react';
import { Layout, Menu, Button, Badge } from 'antd';
import { UserOutlined, LogoutOutlined, MessageOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { AuthService } from '../services/authService';
import { PrivateMessage } from '../components/PrivateMessage';

const { Header, Content, Sider } = Layout;

/**
 * 主页组件
 */
const Home: React.FC = () => {
  const navigate = useNavigate();
  const [messageVisible, setMessageVisible] = useState(false);

  const handleLogout = () => {
    AuthService.logout();
    navigate('/login');
  };

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Header style={{ padding: '0 16px', display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        <h1 style={{ color: 'white', margin: 0 }}>Chat App</h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
          <Badge count={5} offset={[-5, 5]}>
            <Button 
              type="text" 
              icon={<MessageOutlined />} 
              onClick={() => setMessageVisible(true)}
              style={{ color: 'white' }}
            >
              私信
            </Button>
          </Badge>
          <Button 
            type="text" 
            icon={<LogoutOutlined />} 
            onClick={handleLogout}
            style={{ color: 'white' }}
          >
            退出登录
          </Button>
        </div>
      </Header>
      <Layout>
        <Sider width={200} style={{ background: '#fff' }}>
          <Menu
            mode="inline"
            defaultSelectedKeys={['1']}
            style={{ height: '100%', borderRight: 0 }}
          >
            <Menu.Item key="1" icon={<UserOutlined />}>
              聊天列表
            </Menu.Item>
          </Menu>
        </Sider>
        <Layout style={{ padding: '24px' }}>
          <Content style={{ background: '#fff', padding: 24, margin: 0, minHeight: 280 }}>
            欢迎使用 Chat App！
          </Content>
        </Layout>
      </Layout>

      <PrivateMessage 
        visible={messageVisible}
        onClose={() => setMessageVisible(false)}
      />
    </Layout>
  );
};

export default Home; 