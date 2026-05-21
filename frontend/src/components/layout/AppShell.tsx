import { Link, Outlet, useLocation } from 'react-router-dom'
import {
  Banknote,
  CalendarDays,
  CheckSquare,
  ClipboardList,
  Clock,
  FileText,
  LayoutDashboard,
  LogOut,
  Megaphone,
  Star,
  User,
  UserCheck,
  Users,
} from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'
import { useAuth } from '@/features/auth/AuthContext'
import { NotificationBell } from '@/features/notifications/NotificationBell'
import { cn } from '@/lib/utils'

type NavItem = {
  to: string
  label: string
  icon: React.ComponentType<{ className?: string }>
  approverOnly?: boolean
  hrOnly?: boolean
}

const nav: NavItem[] = [
  { to: '/', label: 'แดชบอร์ด', icon: LayoutDashboard },
  { to: '/profile', label: 'โปรไฟล์', icon: User },
  { to: '/announcements', label: 'ประกาศ', icon: Megaphone },
  { to: '/leave', label: 'คำขอลา', icon: CalendarDays },
  { to: '/calendar', label: 'ปฏิทินลา', icon: CalendarDays },
  { to: '/attendance', label: 'ลงเวลา', icon: Clock },
  { to: '/overtime', label: 'ล่วงเวลา', icon: Clock },
  { to: '/expenses', label: 'เบิกค่าใช้จ่าย', icon: Banknote },
  { to: '/payslips', label: 'สลิปเงินเดือน', icon: FileText },
  { to: '/onboarding', label: 'Onboarding', icon: UserCheck },
  { to: '/reviews', label: 'ประเมินผลงาน', icon: Star },
  { to: '/approvals', label: 'รออนุมัติ', icon: CheckSquare, approverOnly: true },
  { to: '/employees', label: 'พนักงาน', icon: Users, hrOnly: true },
  { to: '/holidays', label: 'วันหยุด', icon: CalendarDays, hrOnly: true },
  { to: '/reports', label: 'รายงาน', icon: ClipboardList, hrOnly: true },
]

function isActive(pathname: string, to: string) {
  if (to === '/') return pathname === '/'
  return pathname === to || pathname.startsWith(`${to}/`)
}

export function AppShell() {
  const { me, logout, isApprover, isHr } = useAuth()
  const location = useLocation()

  const visibleNav = nav.filter((item) => {
    if (item.approverOnly && !isApprover) return false
    if (item.hrOnly && !isHr) return false
    return true
  })

  return (
    <div className="flex min-h-svh">
      <aside className="w-56 border-r bg-card flex flex-col shrink-0">
        <div className="p-4 flex items-start justify-between gap-2">
          <div>
            <h1 className="font-semibold text-lg">HR-Lite</h1>
            <p className="text-muted-foreground text-xs mt-1">ระบบภายในองค์กร</p>
          </div>
          <NotificationBell />
        </div>
        <Separator />
        <nav className="flex-1 p-2 space-y-1 overflow-y-auto">
          {visibleNav.map(({ to, label, icon: Icon }) => (
            <Link
              key={to}
              to={to}
              className={cn(
                'flex items-center gap-2 rounded-md px-3 py-2 text-sm transition-colors',
                isActive(location.pathname, to)
                  ? 'bg-primary text-primary-foreground'
                  : 'hover:bg-muted'
              )}
            >
              <Icon className="size-4 shrink-0" />
              {label}
            </Link>
          ))}
        </nav>
        <div className="p-4 border-t space-y-2">
          <p className="text-sm font-medium truncate">{me?.fullName}</p>
          <p className="text-xs text-muted-foreground">{me?.role}</p>
          <Button variant="outline" size="sm" className="w-full" onClick={logout}>
            <LogOut className="size-4" />
            ออกจากระบบ
          </Button>
        </div>
      </aside>
      <main className="flex-1 p-6 overflow-auto">
        <Outlet />
      </main>
    </div>
  )
}
