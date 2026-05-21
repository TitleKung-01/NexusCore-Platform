export function DashboardPanel({
  secretData,
  rateLimitCount,
  users,
  onFetchSecret,
  onFetchUsers,
  onLogout,
}) {
  return (
    <div className="bg-gray-800 p-8 rounded-xl shadow-2xl border border-gray-700 w-full max-w-2xl text-center">
      <h2 className="text-2xl font-bold mb-2 text-lime-400">ยินดีต้อนรับ</h2>
      <p className="text-sm text-gray-400 mb-6">JWT แนบอัตโนมัติผ่าน Axios interceptor</p>

      <div className="flex flex-wrap gap-3 justify-center">
        <button
          type="button"
          onClick={onFetchSecret}
          className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-3 px-6 rounded-lg transition"
        >
          ดึงข้อมูลลับ
        </button>
        <button
          type="button"
          onClick={onFetchUsers}
          className="bg-purple-500 hover:bg-purple-600 text-white font-bold py-3 px-6 rounded-lg transition"
        >
          รายการผู้ใช้ (REST)
        </button>
        <button
          type="button"
          onClick={onLogout}
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
  );
}
