import { useState } from 'react';
import { api, getStoredToken, setStoredToken } from './api';

function App() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState(getStoredToken() || '');
  const [secretData, setSecretData] = useState('');
  const [users, setUsers] = useState([]);
  const [error, setError] = useState('');
  const [rateLimitCount, setRateLimitCount] = useState(0);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    try {
      const response = await api.post('/api/auth/login', { username, password });
      const jwtToken = response.data.token;
      setToken(jwtToken);
      setStoredToken(jwtToken);
    } catch (err) {
      const msg = err.response?.data?.message
        ?? err.response?.data?.errors
        ?? 'เกิดข้อผิดพลาดในการเชื่อมต่อ';
      setError(typeof msg === 'object' ? JSON.stringify(msg) : msg);
    }
  };

  const fetchSecretData = async () => {
    setError('');
    try {
      const response = await api.get('/api/auth/secret-data');
      setSecretData(response.data.data);
      setRateLimitCount((prev) => prev + 1);
    } catch (err) {
      if (err.response?.status === 429) {
        setError('โดนบล็อก — เรียก API ถี่เกินไป (Rate Limit)');
      } else {
        setError('ไม่มีสิทธิ์เข้าถึง หรือตั๋วหมดอายุ (401)');
        if (err.response?.status === 401) setToken('');
      }
    }
  };

  const fetchUsers = async () => {
    setError('');
    try {
      const response = await api.get('/api/users');
      setUsers(response.data);
    } catch (err) {
      setError(err.response?.data?.message ?? 'ไม่สามารถโหลดรายการผู้ใช้ได้');
    }
  };

  const handleLogout = () => {
    setToken('');
    setSecretData('');
    setUsers([]);
    setStoredToken(null);
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-6">
      <h1 className="text-4xl font-bold text-lime-400 mb-8">NexusCore Platform</h1>

      {!token ? (
        <div className="bg-gray-800 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-md">
          <h2 className="text-2xl font-bold mb-6 text-center">ตรวจสอบสิทธิ์เข้าใช้งาน</h2>
          <form onSubmit={handleLogin} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-400">Username (admin)</label>
              <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none focus:border-lime-400"
                required
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-400">Password (password123)</label>
              <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none focus:border-lime-400"
                required
              />
            </div>
            <button
              type="submit"
              className="w-full bg-lime-400 hover:bg-lime-500 text-black font-bold py-2 rounded transition duration-200"
            >
              เข้าสู่ระบบ
            </button>
          </form>
        </div>
      ) : (
        <div className="bg-gray-800 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-2xl text-center">
          <h2 className="text-2xl font-bold mb-2 text-lime-400">ยินดีต้อนรับ</h2>
          <p className="text-sm text-gray-400 mb-6">JWT แนบอัตโนมัติผ่าน Axios interceptor</p>

          <div className="flex flex-wrap gap-3 justify-center">
            <button
              onClick={fetchSecretData}
              className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-3 px-6 rounded-lg transition"
            >
              ดึงข้อมูลลับ
            </button>
            <button
              onClick={fetchUsers}
              className="bg-purple-500 hover:bg-purple-600 text-white font-bold py-3 px-6 rounded-lg transition"
            >
              รายการผู้ใช้ (REST)
            </button>
            <button
              onClick={handleLogout}
              className="bg-red-500 hover:bg-red-600 text-white font-bold py-3 px-6 rounded-lg transition"
            >
              ออกจากระบบ
            </button>
          </div>

          {secretData && (
            <div className="mt-6 p-4 bg-gray-700 text-green-300 rounded-lg border border-green-500">
              {secretData}
              <span className="block text-xs text-gray-400 mt-2">
                (secret-data ครั้งที่ {rateLimitCount})
              </span>
            </div>
          )}

          {users.length > 0 && (
            <ul className="mt-6 text-left p-4 bg-gray-700 rounded-lg border border-gray-600 space-y-2">
              {users.map((u) => (
                <li key={u.id} className="text-sm">
                  <span className="text-lime-300">{u.username}</span>
                  <span className="text-gray-400"> — {u.role}</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {error && (
        <div className="mt-6 p-4 bg-red-900 border border-red-500 text-red-200 rounded-lg max-w-md w-full text-center font-semibold">
          {error}
        </div>
      )}
    </div>
  );
}

export default App;
