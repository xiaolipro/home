import React from 'react';
import { Card, Tabs } from 'antd';
import { FriendshipList } from '../components/FriendshipList';
import { FriendSearch } from '../components/FriendSearch';

export const Friends: React.FC = () => {
    return (
        <Card>
            <Tabs
                items={[
                    {
                        key: 'friends',
                        label: '我的好友',
                        children: <FriendshipList />
                    },
                    {
                        key: 'search',
                        label: '添加好友',
                        children: <FriendSearch />
                    }
                ]}
            />
        </Card>
    );
}; 