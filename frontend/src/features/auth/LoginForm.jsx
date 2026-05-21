import { useState } from 'react';

export function LoginForm({ onLogin }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    await onLogin(username, password);
  };

  return (
    <div className="bg-gray-800 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-md">
      <h2 className="text-2xl font-bold mb-6 text-center">ตรวจสอบสิทธิ์เข้าใช้งาน</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
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
  );
}
