import { useState } from 'react';
import { api } from '../../api';

export function useUsers() {
  const [users, setUsers] = useState([]);

  const fetchUsers = async () => {
    try {
      const response = await api.get('/api/users');
      setUsers(response.data);
      return null;
    } catch (err) {
      return err.response?.data?.message ?? 'ไม่สามารถโหลดรายการผู้ใช้ได้';
    }
  };

  const clearUsers = () => setUsers([]);

  return { users, fetchUsers, clearUsers };
}
