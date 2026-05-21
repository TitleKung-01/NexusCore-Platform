import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { 
  ArrowRight, 
  Calendar, 
  CheckCircle, 
  ChevronRight, 
  Clock, 
  FileText, 
  Megaphone, 
  Palmtree, 
  Sparkles, 
  Stethoscope, 
  UserCheck 
} from 'lucide-react'
import { api } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { useAuth } from '@/features/auth/AuthContext'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { Announcement, ExpenseClaim, LeaveBalance, LeaveRequest, OvertimeRequest } from '@/types/api'

export function DashboardPage() {
  const { me, isApprover } = useAuth()
  const [mine, setMine] = useState<LeaveRequest[]>([])
  const [balances, setBalances] = useState<LeaveBalance[]>([])
  const [announcements, setAnnouncements] = useState<Announcement[]>([])
  const [pendingCount, setPendingCount] = useState(0)

  useEffect(() => {
    api.get<LeaveRequest[]>('/api/leave-requests?scope=mine').then((r) => setMine(r.data.slice(0, 5)))
    api.get<LeaveBalance[]>('/api/leave-balances').then((r) => setBalances(r.data)).catch(() => setBalances([]))
    api
      .get<Announcement[]>('/api/announcements')
      .then((r) => setAnnouncements(r.data.filter((a) => a.isActive).slice(0, 3)))
      .catch(() => setAnnouncements([]))
    if (isApprover) {
      Promise.all([
        api.get<LeaveRequest[]>('/api/leave-requests?scope=pending-approval'),
        api.get<OvertimeRequest[]>('/api/overtime-requests?scope=pending-approval'),
        api.get<ExpenseClaim[]>('/api/expense-claims?scope=pending-approval'),
      ]).then(([leave, ot, exp]) => {
        setPendingCount(leave.data.length + ot.data.length + exp.data.length)
      }).catch(() => setPendingCount(0))
    }
  }, [isApprover])

  // Map leave type to premium icons and colors
  const getLeaveTypeStyle = (name: string) => {
    const n = name.toLowerCase()
    if (n.includes('พักร้อน') || n.includes('vacation')) {
      return {
        icon: Palmtree,
        color: 'from-amber-400 to-orange-500',
        textColor: 'text-orange-600',
        bgColor: 'bg-orange-50 dark:bg-orange-500/10 border-orange-500/10'
      }
    }
    if (n.includes('ป่วย') || n.includes('sick')) {
      return {
        icon: Stethoscope,
        color: 'from-emerald-400 to-teal-500',
        textColor: 'text-teal-600',
        bgColor: 'bg-teal-50 dark:bg-teal-500/10 border-teal-500/10'
      }
    }
    if (n.includes('กิจ') || n.includes('personal')) {
      return {
        icon: FileText,
        color: 'from-blue-400 to-indigo-500',
        textColor: 'text-indigo-600',
        bgColor: 'bg-indigo-50 dark:bg-indigo-500/10 border-indigo-500/10'
      }
    }
    return {
      icon: Sparkles,
      color: 'from-sky-400 to-blue-500',
      textColor: 'text-blue-600',
      bgColor: 'bg-blue-50 dark:bg-blue-500/10 border-blue-500/10'
    }
  }

  // Get current date string in Thai format
  const getThaiDateString = () => {
    const options: Intl.DateTimeFormatOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' }
    return new Date().toLocaleDateString('th-TH', options)
  }

  return (
    <div className="space-y-7">
      
      {/* 👑 Executive Welcome Greeting Banner */}
      <div className="relative overflow-hidden rounded-2xl bg-gradient-to-r from-blue-600 via-indigo-600 to-sky-500 text-white p-6 md:p-8 shadow-xl shadow-blue-500/15">
        {/* Background shapes */}
        <div className="absolute top-0 right-0 w-64 h-64 bg-white/5 rounded-full blur-2xl -mr-16 -mt-16 pointer-events-none" />
        <div className="absolute bottom-0 right-1/4 w-32 h-32 bg-sky-400/10 rounded-full blur-xl pointer-events-none" />
        
        <div className="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div className="space-y-1.5">
            <span className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-white/10 text-xs font-semibold backdrop-blur-md text-sky-100">
              <Sparkles className="size-3 text-amber-300 animate-spin" />
              ยินดีต้อนรับกลับเข้าสู่ระบบ
            </span>
            <h2 className="text-3xl font-extrabold tracking-tight">สวัสดี, คุณ{me?.fullName}</h2>
            <p className="text-sky-100 text-sm font-medium">
              แผนก {me?.departmentName || 'ไม่ได้ระบุ'} · ตำแหน่ง {me?.role || 'พนักงาน'}
            </p>
          </div>
          
          <div className="shrink-0 flex items-center gap-3 bg-white/10 backdrop-blur-md border border-white/10 p-3.5 rounded-xl text-sky-50 md:self-center">
            <Clock className="size-5 text-sky-200 animate-pulse" />
            <div>
              <p className="text-[10px] font-bold uppercase tracking-wider text-sky-200">วันเวลาปฏิบัติงาน</p>
              <p className="text-xs font-bold">{getThaiDateString()}</p>
            </div>
          </div>
        </div>
      </div>

      {/* 🚀 Active Pending Approvals Block (If Manager/Approver) */}
      {isApprover && pendingCount > 0 && (
        <div className="relative overflow-hidden rounded-2xl bg-gradient-to-r from-amber-50 to-orange-50 dark:from-amber-950/20 dark:to-orange-950/20 border border-amber-500/20 p-5 shadow-sm animate-pulse">
          <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div className="flex items-center gap-3">
              <div className="size-11 rounded-xl bg-gradient-to-br from-amber-400 to-orange-500 flex items-center justify-center text-white shadow-md shadow-orange-500/20 shrink-0">
                <UserCheck className="size-5.5" />
              </div>
              <div>
                <h3 className="font-bold text-foreground text-base">มีรายการรอคุณพิจารณาอนุมัติ</h3>
                <p className="text-muted-foreground text-xs font-medium">คำขอลา, ล่วงเวลา, หรือค่าใช้จ่าย รอการตัดสินใจจากคุณทั้งหมด {pendingCount} รายการ</p>
              </div>
            </div>
            <Button className="font-semibold shadow-md bg-gradient-to-r from-amber-500 to-orange-600 hover:from-amber-600 hover:to-orange-700 text-white rounded-xl border-0 h-10 shrink-0" asChild>
              <Link to="/approvals" className="flex items-center gap-1.5">
                ไปหน้าอนุมัติ <ArrowRight className="size-4" />
              </Link>
            </Button>
          </div>
        </div>
      )}

      {/* 📅 Leave balances (โควแต่วันลาคงเหลือ) */}
      {balances.length > 0 && (
        <div className="space-y-3.5">
          <div className="flex items-center justify-between">
            <h3 className="font-bold text-lg text-foreground tracking-tight flex items-center gap-2">
              <span className="size-2 rounded-full bg-blue-500" />
              วันลาคงเหลือและสิทธิ์การใช้งาน
            </h3>
          </div>
          
          <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
            {balances.map((b) => {
              const style = getLeaveTypeStyle(b.leaveTypeName)
              const Icon = style.icon
              const percentage = b.daysAllowed > 0 
                ? Math.min(100, Math.max(0, (b.daysRemaining / b.daysAllowed) * 100))
                : 0
                
              return (
                <Card key={b.leaveTypeId} className={`overflow-hidden border shadow-sm hover:shadow-md transition-all duration-300 hover:scale-[1.01] bg-card ${style.bgColor}`}>
                  <CardContent className="p-5 space-y-4">
                    <div className="flex justify-between items-start">
                      <div className="space-y-1">
                        <p className="text-xs font-semibold text-muted-foreground tracking-wide">{b.leaveTypeName}</p>
                        <h4 className="text-3xl font-extrabold tracking-tight text-foreground">
                          {b.daysRemaining} <span className="text-sm font-semibold text-muted-foreground">วัน</span>
                        </h4>
                      </div>
                      <div className={`size-10 rounded-xl bg-gradient-to-br ${style.color} flex items-center justify-center text-white shadow-md`}>
                        <Icon className="size-5" />
                      </div>
                    </div>
                    
                    {/* Visual Progress Bar */}
                    <div className="space-y-1.5">
                      <div className="flex justify-between text-[10px] font-bold text-muted-foreground uppercase">
                        <span>ใช้ไปแล้ว {b.daysUsed} วัน</span>
                        <span>สิทธิ์ทั้งหมด {b.daysAllowed} วัน</span>
                      </div>
                      <div className="w-full h-2 rounded-full bg-blue-500/10 dark:bg-white/10 overflow-hidden">
                        <div 
                          className={`h-full rounded-full bg-gradient-to-r ${style.color} transition-all duration-500`}
                          style={{ width: `${percentage}%` }}
                        />
                      </div>
                    </div>
                  </CardContent>
                </Card>
              )
            })}
          </div>
        </div>
      )}

      {/* Bottom Layout Split (Announcements & Recent Leave Requests) */}
      <div className="grid gap-6 lg:grid-cols-5">
        
        {/* 📢 Announcements (ประกาศองค์กรล่าสุด) */}
        {announcements.length > 0 && (
          <Card className="lg:col-span-2 shadow-sm border overflow-hidden">
            <CardHeader className="flex flex-row items-center justify-between pb-3 border-b border-muted/50 bg-gradient-to-r from-transparent to-blue-500/[0.01]">
              <CardTitle className="text-base font-bold flex items-center gap-2 text-foreground">
                <Megaphone className="size-4.5 text-blue-500 animate-bounce" />
                ประกาศล่าสุด
              </CardTitle>
              <Button variant="ghost" size="sm" className="font-semibold text-xs text-blue-600 hover:text-blue-700 hover:bg-blue-50" asChild>
                <Link to="/announcements" className="flex items-center">
                  ดูทั้งหมด <ChevronRight className="size-3.5" />
                </Link>
              </Button>
            </CardHeader>
            <CardContent className="p-4 space-y-3.5">
              {announcements.map((a) => (
                <div key={a.id} className="p-3.5 rounded-xl border border-blue-500/5 hover:border-blue-500/10 bg-blue-500/[0.01] hover:bg-blue-500/[0.03] transition-all space-y-1">
                  <div className="flex items-center gap-2">
                    <span className="size-1.5 rounded-full bg-blue-500" />
                    <p className="font-bold text-sm text-foreground truncate">{a.title}</p>
                  </div>
                  <p className="text-xs text-muted-foreground line-clamp-2 leading-relaxed ml-3.5">
                    {a.body}
                  </p>
                </div>
              ))}
            </CardContent>
          </Card>
        )}

        {/* 📝 Recent Leave Requests (คำขอลาล่าสุด) */}
        <Card className={announcements.length > 0 ? "lg:col-span-3 shadow-sm border overflow-hidden" : "lg:col-span-5 shadow-sm border overflow-hidden"}>
          <CardHeader className="flex flex-row items-center justify-between pb-3 border-b border-muted/50 bg-gradient-to-r from-transparent to-blue-500/[0.01]">
            <CardTitle className="text-base font-bold flex items-center gap-2 text-foreground">
              <Calendar className="size-4.5 text-blue-500" />
              คำขอลาล่าสุดของฉัน
            </CardTitle>
            <Button variant="ghost" size="sm" className="font-semibold text-xs text-blue-600 hover:text-blue-700 hover:bg-blue-50" asChild>
              <Link to="/leave" className="flex items-center">
                ดูทั้งหมด <ChevronRight className="size-3.5" />
              </Link>
            </Button>
          </CardHeader>
          <CardContent className="p-4 space-y-3.5">
            {mine.map((item) => (
              <div key={item.id} className="flex items-center justify-between p-3.5 rounded-xl border border-blue-500/5 hover:border-blue-500/10 bg-blue-500/[0.01] hover:bg-blue-500/[0.02] transition-all">
                <div className="space-y-1">
                  <p className="font-bold text-sm text-foreground">{item.leaveTypeName}</p>
                  <p className="text-xs text-muted-foreground flex items-center gap-1.5">
                    <Clock className="size-3" />
                    {item.startDate} — {item.endDate}
                  </p>
                </div>
                <div className="flex items-center gap-3">
                  <LeaveStatusBadge status={item.status} />
                  <Button variant="outline" size="sm" className="size-8 p-0 rounded-lg shrink-0 border-blue-500/10 text-muted-foreground hover:text-blue-600 hover:bg-blue-50" asChild>
                    <Link to={`/leave/${item.id}`}>
                      <ChevronRight className="size-4" />
                    </Link>
                  </Button>
                </div>
              </div>
            ))}
            
            {mine.length === 0 && (
              <div className="py-8 text-center space-y-3.5">
                <div className="mx-auto size-12 rounded-full bg-blue-50 flex items-center justify-center text-blue-500">
                  <CheckCircle className="size-6" />
                </div>
                <p className="text-muted-foreground text-sm font-medium">
                  ไม่มีประวัติการส่งคำขอลาในขณะนี้
                </p>
                <Button variant="outline" size="sm" className="border-blue-500/20 text-blue-600 rounded-xl hover:bg-blue-50 font-semibold" asChild>
                  <Link to="/leave/new">ยื่นคำขอลาใหม่</Link>
                </Button>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

