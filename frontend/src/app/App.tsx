import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Toaster } from 'sonner'
import { AppShell } from '@/components/layout/AppShell'
import { AuthProvider, useAuth } from '@/features/auth/AuthContext'
import { LoginPage } from '@/features/auth/LoginPage'
import { ApprovalsPage } from '@/features/approvals/ApprovalsPage'
import { DashboardPage } from '@/features/dashboard/DashboardPage'
import { LeaveDetailPage } from '@/features/leave/LeaveDetailPage'
import { LeaveListPage } from '@/features/leave/LeaveListPage'
import { LeaveNewPage } from '@/features/leave/LeaveNewPage'
import { ProfilePage } from '@/features/profile/ProfilePage'

function RequireAuth({ children }: { children: React.ReactNode }) {
  const { token, loading } = useAuth()
  if (!token) return <Navigate to="/login" replace />
  if (loading) return <div className="p-8 text-muted-foreground">กำลังโหลด...</div>
  return <>{children}</>
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Toaster richColors position="top-right" />
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            element={
              <RequireAuth>
                <AppShell />
              </RequireAuth>
            }
          >
            <Route index element={<DashboardPage />} />
            <Route path="profile" element={<ProfilePage />} />
            <Route path="leave" element={<LeaveListPage />} />
            <Route path="leave/new" element={<LeaveNewPage />} />
            <Route path="leave/:id" element={<LeaveDetailPage />} />
            <Route path="approvals" element={<ApprovalsPage />} />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
