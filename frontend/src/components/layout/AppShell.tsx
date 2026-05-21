import { Link, Outlet, useLocation } from 'react-router-dom'
import { CalendarDays, CheckSquare, LayoutDashboard, LogOut, User } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'
import { useAuth } from '@/features/auth/AuthContext'
import { cn } from '@/lib/utils'

const nav = [
  { to: '/', label: 'แดชบอร์ด', icon: LayoutDashboard },
  { to: '/profile', label: 'โปรไฟล์', icon: User },
  { to: '/leave', label: 'คำขอลา', icon: CalendarDays },
  { to: '/approvals', label: 'รออนุมัติ', icon: CheckSquare, approverOnly: true },
]

export function AppShell() {
  const { me, logout, isApprover } = useAuth()
  const location = useLocation()

  return (
    <div className="flex min-h-svh">
      <aside className="w-56 border-r bg-card flex flex-col">
        <div className="p-4">
          <h1 className="font-semibold text-lg">HR-Lite</h1>
          <p className="text-muted-foreground text-xs mt-1">ระบบภายในองค์กร</p>
        </div>
        <Separator />
        <nav className="flex-1 p-2 space-y-1">
          {nav
            .filter((item) => !item.approverOnly || isApprover)
            .map(({ to, label, icon: Icon }) => (
              <Link
                key={to}
                to={to}
                className={cn(
                  'flex items-center gap-2 rounded-md px-3 py-2 text-sm transition-colors',
                  location.pathname === to || (to !== '/' && location.pathname.startsWith(to))
                    ? 'bg-primary text-primary-foreground'
                    : 'hover:bg-muted'
                )}
              >
                <Icon className="size-4" />
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
