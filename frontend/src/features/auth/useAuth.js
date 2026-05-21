import { useState } from 'react';
import { api, getStoredToken, setStoredToken } from '../../api';

function formatApiError(err, fallback) {
  const msg = err.response?.data?.message
    ?? err.response?.data?.errors
    ?? fallback;
  return typeof msg === 'object' ? JSON.stringify(msg) : msg;
}

export function useAuth() {
  const [token, setToken] = useState(getStoredToken() || '');
  const [secretData, setSecretData] = useState('');
  const [rateLimitCount, setRateLimitCount] = useState(0);

  const login = async (username, password) => {
    try {
      const response = await api.post('/api/auth/login', { username, password });
      const jwtToken = response.data.token;
      setToken(jwtToken);
      setStoredToken(jwtToken);
      return null;
    } catch (err) {
      return formatApiError(err, 'เกิดข้อผิดพลาดในการเชื่อมต่อ');
    }
  };

  const logout = () => {
    setToken('');
    setSecretData('');
    setRateLimitCount(0);
    setStoredToken(null);
  };

  const fetchSecretData = async () => {
    try {
      const response = await api.get('/api/auth/secret-data');
      setSecretData(response.data.data);
      setRateLimitCount((prev) => prev + 1);
      return null;
    } catch (err) {
      if (err.response?.status === 429) {
        return 'โดนบล็อก — เรียก API ถี่เกินไป (Rate Limit)';
      }
      if (err.response?.status === 401) {
        setToken('');
      }
      return 'ไม่มีสิทธิ์เข้าถึง หรือตั๋วหมดอายุ (401)';
    }
  };

  return {
    token,
    secretData,
    rateLimitCount,
    login,
    logout,
    fetchSecretData,
  };
}
