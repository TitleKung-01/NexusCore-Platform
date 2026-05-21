import { useEffect, useMemo, useState } from 'react'
import { CalendarDays, CalendarRange, Palmtree, Sparkles } from 'lucide-react'
import { api, formatApiError } from '@/api'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import {
  filterBarClass,
  PageError,
  PageHeader,
  PageShell,
  premiumCardClass,
} from '@/features/shared/PageHeader'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import { useAuth } from '@/features/auth/AuthContext'
import type { Department, Holiday, LeaveCalendarEntry } from '@/types/api'

function monthRange(offset: number) {
  const d = new Date()
  d.setMonth(d.getMonth() + offset, 1)
  const from = `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-01`
  const end = new Date(d.getFullYear(), d.getMonth() + 1, 0)
  const to = `${end.getFullYear()}-${String(end.getMonth() + 1).padStart(2, '0')}-${String(end.getDate()).padStart(2, '0')}`
  return { from, to, year: d.getFullYear() }
}

export function CalendarPage() {
  const { isHr } = useAuth()
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [departmentId, setDepartmentId] = useState('')
  const [departments, setDepartments] = useState<Department[]>([])
  const [entries, setEntries] = useState<LeaveCalendarEntry[]>([])
  const [holidays, setHolidays] = useState<Holiday[]>([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const year = useMemo(() => (from ? Number(from.slice(0, 4)) : new Date().getFullYear()), [from])

  const approvedCount = useMemo(
    () => entries.filter((e) => e.status === 'Approved').length,
    [entries]
  )

  useEffect(() => {
    const { from: f, to: t } = monthRange(0)
    setFrom(f)
    setTo(t)
  }, [])

  useEffect(() => {
    if (isHr) {
      api.get<Department[]>('/api/departments').then((r) => setDepartments(r.data)).catch(() => setDepartments([]))
    }
  }, [isHr])

  useEffect(() => {
    if (!from || !to) return
    setError('')
    setLoading(true)
    const params: Record<string, string> = { from, to }
    if (departmentId) params.departmentId = departmentId
    Promise.all([
      api.get<LeaveCalendarEntry[]>('/api/leave-requests/calendar', { params }),
      api.get<Holiday[]>('/api/holidays', { params: { year } }),
    ])
      .then(([calRes, holRes]) => {
        setEntries(calRes.data)
        setHolidays(holRes.data)
      })
      .catch((e) => {
        setEntries([])
        setError(formatApiError(e, 'โหลดปฏิทินไม่สำเร็จ'))
      })
      .finally(() => setLoading(false))
  }, [from, to, departmentId, year])

  const setQuickRange = (offset: number) => {
    const { from: f, to: t } = monthRange(offset)
    setFrom(f)
    setTo(t)
  }

  return (
    <PageShell>
      <PageHeader
        icon={<CalendarRange className="size-7" />}
        title="ปฏิทินลา"
        description="ดูวันลาของทีมและวันหยุดประจำปีในช่วงวันที่ที่เลือก"
      />

      <PageError message={error} />

      <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
        <div className="rounded-xl border border-blue-500/10 bg-blue-500/5 p-4 flex items-center justify-between">
          <div>
            <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">รายการลา</p>
            <p className="text-2xl font-black mt-0.5">{loading ? '—' : entries.length}</p>
          </div>
          <CalendarDays className="size-8 text-primary opacity-80" />
        </div>
        <div className="rounded-xl border border-emerald-500/10 bg-emerald-500/5 p-4 flex items-center justify-between">
          <div>
            <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">อนุมัติแล้ว</p>
            <p className="text-2xl font-black mt-0.5">{loading ? '—' : approvedCount}</p>
          </div>
          <Sparkles className="size-8 text-emerald-600 opacity-80" />
        </div>
        <div className="rounded-xl border border-amber-500/10 bg-amber-500/5 p-4 flex items-center justify-between col-span-2 sm:col-span-1">
          <div>
            <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">วันหยุด {year}</p>
            <p className="text-2xl font-black mt-0.5">{loading ? '—' : holidays.length}</p>
          </div>
          <Palmtree className="size-8 text-amber-600 opacity-80" />
        </div>
      </div>

      <div className={filterBarClass}>
        <div className="space-y-2">
          <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">จากวันที่</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="rounded-xl w-[160px]" />
        </div>
        <div className="space-y-2">
          <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">ถึงวันที่</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="rounded-xl w-[160px]" />
        </div>
        {isHr && (
          <div className="space-y-2 min-w-[200px]">
            <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">แผนก</Label>
            <Select value={departmentId || 'all'} onValueChange={(v) => setDepartmentId(v === 'all' ? '' : v)}>
              <SelectTrigger className="rounded-xl">
                <SelectValue placeholder="ทุกแผนก" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">ทุกแผนก</SelectItem>
                {departments.map((d) => (
                  <SelectItem key={d.id} value={d.id}>
                    {d.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        )}
        <div className="flex gap-2 ml-auto">
          <button
            type="button"
            onClick={() => setQuickRange(0)}
            className="text-xs font-semibold px-3 py-2 rounded-lg bg-primary/10 text-primary hover:bg-primary/15 transition-colors"
          >
            เดือนนี้
          </button>
          <button
            type="button"
            onClick={() => setQuickRange(1)}
            className="text-xs font-semibold px-3 py-2 rounded-lg bg-muted hover:bg-muted/80 transition-colors"
          >
            เดือนถัดไป
          </button>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <CalendarDays className="size-5" />
              วันลาในช่วงที่เลือก
            </CardTitle>
            <CardDescription>รายการลาทั้งหมดในช่วงวันที่กำหนด</CardDescription>
          </CardHeader>
          <CardContent className="p-0">
            <Table>
              <TableHeader>
                <TableRow className="bg-muted/30 hover:bg-muted/30">
                  <TableHead className="font-bold">พนักงาน</TableHead>
                  <TableHead className="font-bold">ประเภท</TableHead>
                  <TableHead className="font-bold">ช่วงวัน</TableHead>
                  <TableHead className="font-bold">สถานะ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {entries.map((e) => (
                  <TableRow key={e.id} className="hover:bg-blue-500/[0.02]">
                    <TableCell className="font-medium">{e.employeeName}</TableCell>
                    <TableCell className="font-medium">{e.leaveTypeName}</TableCell>
                    <TableCell className="text-sm">
                      {e.startDate} — {e.endDate}
                    </TableCell>
                    <TableCell>
                      <WorkflowStatusBadge status={e.status} />
                    </TableCell>
                  </TableRow>
                ))}
                {!loading && entries.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} className="text-center text-muted-foreground py-10">
                      ไม่มีรายการลาในช่วงนี้
                    </TableCell>
                  </TableRow>
                )}
                {loading && (
                  <TableRow>
                    <TableCell colSpan={4} className="text-center text-muted-foreground py-10">
                      กำลังโหลด...
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <Palmtree className="size-5" />
              วันหยุดประจำปี {year}
            </CardTitle>
            <CardDescription>วันหยุดนักขัตฤกษ์และวันหยุดบริษัท</CardDescription>
          </CardHeader>
          <CardContent className="p-4">
            <ul className="space-y-2">
              {holidays.map((h) => (
                <li
                  key={h.id}
                  className="flex justify-between items-center gap-3 rounded-xl border border-border/40 bg-muted/20 px-4 py-3 text-sm hover:bg-muted/40 transition-colors"
                >
                  <span className="font-semibold">{h.name}</span>
                  <span className="text-muted-foreground font-medium tabular-nums shrink-0">{h.date}</span>
                </li>
              ))}
              {!loading && holidays.length === 0 && (
                <p className="text-center text-muted-foreground py-8 text-sm">ไม่มีวันหยุดในปีนี้</p>
              )}
            </ul>
          </CardContent>
        </Card>
      </div>
    </PageShell>
  )
}
