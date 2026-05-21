import { useState } from 'react';
import { useAuth } from './features/auth/useAuth';
import { LoginForm } from './features/auth/LoginForm';
import { DashboardPanel } from './features/auth/DashboardPanel';
import { useUsers } from './features/users/useUsers';
import { ErrorAlert } from './shared/components/ErrorAlert';

function App() {
  const [error, setError] = useState('');
  const {
    token,
    secretData,
    rateLimitCount,
    login,
    logout,
    fetchSecretData,
  } = useAuth();
  const { users, fetchUsers, clearUsers } = useUsers();

  const handleLogin = async (username, password) => {
    setError('');
    const err = await login(username, password);
    if (err) setError(err);
  };

  const handleFetchSecret = async () => {
    setError('');
    const err = await fetchSecretData();
    if (err) setError(err);
  };

  const handleFetchUsers = async () => {
    setError('');
    const err = await fetchUsers();
    if (err) setError(err);
  };

  const handleLogout = () => {
    logout();
    clearUsers();
    setError('');
  };

  return (
    <div className="min-h-screen bg-gray-900 text-white flex flex-col items-center justify-center p-6">
      <h1 className="text-4xl font-bold text-lime-400 mb-8">NexusCore Platform</h1>

      {!token ? (
        <LoginForm onLogin={handleLogin} />
      ) : (
        <DashboardPanel
          secretData={secretData}
          rateLimitCount={rateLimitCount}
          users={users}
          onFetchSecret={handleFetchSecret}
          onFetchUsers={handleFetchUsers}
          onLogout={handleLogout}
        />
      )}

      <ErrorAlert message={error} />
    </div>
  );
}

export default App;
