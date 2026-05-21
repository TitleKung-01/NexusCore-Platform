import { useState } from 'react';
import { api } from './api';

function App() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState(localStorage.getItem('jwt_token') || '');
  const [secretData, setSecretData] = useState('');
  const [error, setError] = useState('');
  const [rateLimitCount, setRateLimitCount] = useState(0);

  // 1. ฟังก์ชันสำหรับส่งข้อมูลไปเข้าสู่ระบบ
  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    try {
      // ยิงไปที่ Gateway (Port 5000) ไม่ได้ยิงตรงไปหลังบ้าน
      const response = await api.post('/api/auth/login', {
        username,
        password
      });
      const jwtToken = response.data.token;
      setToken(jwtToken);
      localStorage.setItem('jwt_token', jwtToken); // เก็บตั๋วไว้ในเครื่อง
    } catch (err) {
      setError(err.response?.data?.message || 'เกิดข้อผิดพลาดในการเชื่อมต่อ');
    }
  };

  // 2. ฟังก์ชันสำหรับใช้ตั๋ว JWT ไปดึงข้อมูลลับผ่าน Gateway
  const fetchSecretData = async () => {
    setError('');
    try {
      const response = await api.get('/api/auth/secret-data', {
        headers: {
          Authorization: `Bearer ${token}` // แนบตั๋ว VIP ไปใน Header
        }
      });
      setSecretData(response.data.data);
      setRateLimitCount(prev => prev + 1);
    } catch (err) {
      if (err.response?.status === 429) {
        setError('🛑 โดนบล็อก! คุณกดถี่เกินไป (Rate Limit ทำงานแล้วงับ)');
      } else {
        setError('🔒 คุณไม่มีสิทธิ์เข้าถึง หรือตั๋วหมดอายุ (401 Unauthorized)');
      }
    }
  };

  const handleLogout = () => {
    setToken('');
    setSecretData('');
    localStorage.removeItem('jwt_token');
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-6">
      <h1 className="text-4xl font-bold text-lime-400 mb-8">NexusCore Platform</h1>

      {!token ? (
        /* --- ฟอร์มเข้าสู่ระบบ (Login Form) --- */
        <div className="bg-gray-850 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-md bg-gray-800">
          <h2 className="text-2xl font-bold mb-6 text-center">🔐 ตรวจสอบสิทธิ์เข้าใช้งาน</h2>
          <form onSubmit={handleLogin} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-400">Username (พิมพ์: admin)</label>
              <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none focus:border-lime-400" required />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-400">Password (พิมพ์: password123)</label>
              <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} className="w-full mt-1 p-2 rounded bg-gray-700 border border-gray-600 focus:outline-none focus:border-lime-400" required />
            </div>
            <button type="submit" className="w-full bg-lime-400 hover:bg-lime-500 text-black font-bold py-2 rounded transition duration-200">เข้าสู่ระบบ</button>
          </form>
        </div>
      ) : (
        /* --- หน้าด่านควบคุมเมื่อล็อกอินผ่านแล้ว (Dashboard) --- */
        <div className="bg-gray-800 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-2xl text-center">
          <h2 className="text-2xl font-bold mb-2 text-lime-400">🎉 ยินดีต้อนรับเข้าสู่ระบบหลังบ้าน</h2>
          <p className="text-sm text-gray-400 mb-6">มีตั๋ว JWT แอบเก็บไว้ในสถานะเรียบร้อย</p>

          <div className="space-y-4">
            <button onClick={fetchSecretData} className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-3 px-6 rounded-lg transition mr-4">
              🎯 ดึงข้อมูลลับระดับองค์กร
            </button>
            <button onClick={handleLogout} className="bg-red-500 hover:bg-red-600 text-white font-bold py-3 px-6 rounded-lg transition">
              ออกจากระบบ
            </button>
          </div>

          {secretData && (
            <div className="mt-6 p-4 bg-gray-700 text-green-300 rounded-lg border border-green-500">
              {secretData} <span className="block text-xs text-gray-450 mt-2 text-gray-400">(เรียกสำเร็จครั้งที่ {rateLimitCount})</span>
            </div>
          )}
        </div>
      )}

      {/* บล็อกแสดง Error หรือแจ้งเตือน Rate Limit */}
      {error && (
        <div className="mt-6 p-4 bg-red-900 border border-red-500 text-red-200 rounded-lg max-w-md w-full text-center font-semibold">
          {error}
        </div>
      )}
    </div>
  );
}

export default App;