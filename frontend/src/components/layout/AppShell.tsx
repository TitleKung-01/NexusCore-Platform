import { Link, Outlet, useLocation } from 'react-router-dom'
import {
  Banknote,
  Building2,
  CalendarDays,
  CheckSquare,
  ClipboardList,
  Clock,
  FileText,
  LayoutDashboard,
  LogOut,
  Megaphone,
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

  // Get user initials for profile avatar
  const getInitials = (name?: string) => {
    if (!name) return 'HR'
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase()
  }

  return (
    <div className="flex min-h-svh bg-background">
      <aside className="w-64 border-r border-blue-500/10 bg-card flex flex-col shrink-0 shadow-lg shadow-blue-500/[0.01] relative z-20">
        {/* Sidebar Header with Brand */}
        <div className="p-4 pt-5 pb-5 flex items-center justify-between gap-2 border-b border-blue-500/5 overflow-visible">
          <Link to="/" className="flex items-center gap-2.5 group">
            <div className="size-9 rounded-xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white shadow-md shadow-blue-500/20 group-hover:scale-105 transition-transform">
              <Building2 className="size-4.5" />
            </div>
            <div>
              <h1 className="font-bold text-base tracking-tight bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent dark:from-blue-400 dark:to-indigo-400">
                HR-Lite
              </h1>
              <p className="text-muted-foreground text-[10px] font-semibold uppercase tracking-wider -mt-0.5">
                Portal องค์กร
              </p>
            </div>
          </Link>
          <NotificationBell />
        </div>
        
        {/* Navigation Section */}
        <nav className="flex-1 p-3 space-y-1 overflow-y-auto scrollbar-thin">
          {visibleNav.map(({ to, label, icon: Icon }) => {
            const active = isActive(location.pathname, to)
            return (
              <Link
                key={to}
                to={to}
                className={cn(
                  'flex items-center gap-2.5 rounded-xl px-3.5 py-2.5 text-sm font-medium transition-all duration-300 relative group',
                  active
                    ? 'bg-gradient-to-r from-blue-500 to-indigo-600 text-white shadow-md shadow-blue-500/20 font-semibold'
                    : 'text-muted-foreground hover:text-blue-600 hover:bg-blue-500/5 dark:hover:bg-blue-500/10'
                )}
              >
                {/* Active Left Indicator Line */}
                {active && (
                  <span className="absolute left-1 top-1/4 bottom-1/4 w-1 rounded-full bg-white" />
                )}
                
                <Icon className={cn(
                  'size-4 shrink-0 transition-transform group-hover:scale-110',
                  active ? 'text-white' : 'text-muted-foreground group-hover:text-blue-500'
                )} />
                <span>{label}</span>
              </Link>
            )
          })}
        </nav>
        
        {/* User Card & Logout Section */}
        <div className="p-4 border-t border-blue-500/5 bg-gradient-to-b from-transparent to-blue-500/[0.02]">
          <div className="bg-blue-500/[0.03] dark:bg-white/[0.02] border border-blue-500/5 p-3 rounded-xl space-y-3 shadow-inner">
            <div className="flex items-center gap-3">
              <div className="size-10 rounded-full bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white text-sm font-bold shadow-md shadow-blue-500/20 border border-white/20">
                {getInitials(me?.fullName)}
              </div>
              <div className="min-w-0 flex-1">
                <p className="text-sm font-semibold truncate text-foreground leading-snug">
                  {me?.fullName}
                </p>
                <p className="text-[10px] font-semibold text-muted-foreground truncate uppercase tracking-wider">
                  {me?.role || 'พนักงาน'}
                </p>
              </div>
            </div>
            
            <Separator className="bg-blue-500/5" />
            
            <Button 
              variant="outline" 
              size="sm" 
              className="w-full justify-center gap-1.5 h-9 rounded-lg border-blue-500/10 text-xs font-semibold text-muted-foreground hover:bg-destructive/10 hover:text-destructive hover:border-destructive/20 hover:scale-[1.01] active:scale-100 transition-all bg-card shadow-xs" 
              onClick={logout}
            >
              <LogOut className="size-3.5" />
              ออกจากระบบ
            </Button>
          </div>
        </div>
      </aside>
      
      {/* Main Screen */}
      <main className="flex-1 p-6 lg:p-8 overflow-auto relative z-10">
        <Outlet />
      </main>
    </div>
  )
}

