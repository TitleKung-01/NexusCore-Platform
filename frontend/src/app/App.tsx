import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { Toaster } from 'sonner'
import { AppShell } from '@/components/layout/AppShell'
import { AuthProvider, useAuth } from '@/features/auth/AuthContext'
import { LoginPage } from '@/features/auth/LoginPage'
import { ApprovalsPage } from '@/features/approvals/ApprovalsPage'
import { AttendancePage } from '@/features/attendance/AttendancePage'
import { AnnouncementsPage } from '@/features/announcements/AnnouncementsPage'
import { CalendarPage } from '@/features/calendar/CalendarPage'
import { DashboardPage } from '@/features/dashboard/DashboardPage'
import { ExpenseDetailPage } from '@/features/expenses/ExpenseDetailPage'
import { ExpenseListPage } from '@/features/expenses/ExpenseListPage'
import { ExpenseNewPage } from '@/features/expenses/ExpenseNewPage'
import { EmployeesPage } from '@/features/hr/EmployeesPage'
import { HolidaysPage } from '@/features/hr/HolidaysPage'
import { ReportsPage } from '@/features/hr/ReportsPage'
import { LeaveDetailPage } from '@/features/leave/LeaveDetailPage'
import { LeaveListPage } from '@/features/leave/LeaveListPage'
import { LeaveNewPage } from '@/features/leave/LeaveNewPage'
import { OnboardingPage } from '@/features/onboarding/OnboardingPage'
import { OvertimeDetailPage } from '@/features/overtime/OvertimeDetailPage'
import { OvertimeListPage } from '@/features/overtime/OvertimeListPage'
import { OvertimeNewPage } from '@/features/overtime/OvertimeNewPage'
import { PayslipsPage } from '@/features/payslips/PayslipsPage'
import { ProfilePage } from '@/features/profile/ProfilePage'
import { ReviewsPage } from '@/features/reviews/ReviewsPage'

function RequireAuth({ children }: { children: React.ReactNode }) {
  const { token, loading } = useAuth()
  if (!token) return <Navigate to="/login" replace />
  if (loading) return <div className="p-8 text-muted-foreground">กำลังโหลด...</div>
  return <>{children}</>
}

function RequireHr({ children }: { children: React.ReactNode }) {
  const { isHr } = useAuth()
  if (!isHr) return <Navigate to="/" replace />
  return <>{children}</>
}

function RequireApprover({ children }: { children: React.ReactNode }) {
  const { isApprover } = useAuth()
  if (!isApprover) return <Navigate to="/" replace />
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
            <Route path="announcements" element={<AnnouncementsPage />} />
            <Route path="leave" element={<LeaveListPage />} />
            <Route path="leave/new" element={<LeaveNewPage />} />
            <Route path="leave/:id" element={<LeaveDetailPage />} />
            <Route path="calendar" element={<CalendarPage />} />
            <Route path="attendance" element={<AttendancePage />} />
            <Route path="overtime" element={<OvertimeListPage />} />
            <Route path="overtime/new" element={<OvertimeNewPage />} />
            <Route path="overtime/:id" element={<OvertimeDetailPage />} />
            <Route path="expenses" element={<ExpenseListPage />} />
            <Route path="expenses/new" element={<ExpenseNewPage />} />
            <Route path="expenses/:id" element={<ExpenseDetailPage />} />
            <Route path="payslips" element={<PayslipsPage />} />
            <Route path="onboarding" element={<OnboardingPage />} />
            <Route path="reviews" element={<ReviewsPage />} />
            <Route
              path="approvals"
              element={
                <RequireApprover>
                  <ApprovalsPage />
                </RequireApprover>
              }
            />
            <Route
              path="employees"
              element={
                <RequireHr>
                  <EmployeesPage />
                </RequireHr>
              }
            />
            <Route
              path="holidays"
              element={
                <RequireHr>
                  <HolidaysPage />
                </RequireHr>
              }
            />
            <Route
              path="reports"
              element={
                <RequireHr>
                  <ReportsPage />
                </RequireHr>
              }
            />
          </Route>
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}
