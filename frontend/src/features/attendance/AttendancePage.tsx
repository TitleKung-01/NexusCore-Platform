import { useCallback, useEffect, useState } from 'react'
import { Clock, History, LogIn, LogOut } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Textarea } from '@/components/ui/textarea'
import { useAuth } from '@/features/auth/AuthContext'
import type { AttendanceRecord } from '@/types/api'
import { normalizeAttendance } from '@/features/attendance/attendanceUtils'
import {
  filterBarClass,
  PageHeader,
  PageShell,
  premiumCardClass,
} from '@/features/shared/PageHeader'

const SCHEDULE_IN = '09:00'
const SCHEDULE_OUT = '18:00'

function todayStr() {
  return new Date().toLocaleDateString('en-CA', { timeZone: 'Asia/Bangkok' })
}

function weekRange() {
  const to = todayStr()
  const d = new Date()
  d.setDate(d.getDate() - 6)
  const from = d.toLocaleDateString('en-CA', { timeZone: 'Asia/Bangkok' })
  return { from, to }
}

function statusBadgeVariant(record: AttendanceRecord | null): 'default' | 'secondary' | 'destructive' | 'outline' {
  if (!record?.checkInUtc) return 'outline'
  if (record.isLateCheckIn || record.isEarlyCheckOut) return 'destructive'
  if (!record.checkOutUtc) return 'secondary'
  return 'default'
}

export function AttendancePage() {
  const { me, isApprover } = useAuth()
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [items, setItems] = useState<AttendanceRecord[]>([])
  const [today, setToday] = useState<AttendanceRecord | null>(null)
  const [workSummary, setWorkSummary] = useState('')
  const [acting, setActing] = useState(false)

  const loadHistory = useCallback(() => {
    if (!from || !to) return
    api
      .get<AttendanceRecord[]>('/api/attendance', { params: { from, to } })
      .then((r) => setItems((r.data ?? []).map((row) => normalizeAttendance(row)!).filter(Boolean)))
      .catch(() => setItems([]))
  }, [from, to])

  const loadToday = useCallback(() => {
    api
      .get<AttendanceRecord | null>('/api/attendance/today')
      .then((r) => setToday(normalizeAttendance(r.data)))
      .catch(() => setToday(null))
  }, [])

  const refresh = useCallback(() => {
    loadToday()
    loadHistory()
  }, [loadToday, loadHistory])

  useEffect(() => {
    const { from: f, to: t } = weekRange()
    setFrom(f)
    setTo(t)
  }, [])

  useEffect(() => {
    loadHistory()
  }, [loadHistory])

  useEffect(() => {
    loadToday()
  }, [loadToday])

  useEffect(() => {
    if (today) return
    const row = items.find((a) => a.workDate === todayStr())
    if (row) setToday(row)
  }, [items, today])

  const canCheckIn = today?.canCheckIn ?? true
  const canCheckOut = today?.canCheckOut ?? false
  const hasCheckIn = Boolean(today?.checkInUtc)
  const hasCheckOut = Boolean(today?.checkOutUtc)

  const checkInHint = canCheckIn
    ? 'ลงเวลาเข้าได้ตลอดวัน — เข้าหลัง 09:00 จะแสดงว่าสาย (ไม่ต้องรอถึง 09:00)'
    : 'ลงเวลาเข้าแล้ววันนี้'
  const checkOutHint = !hasCheckIn
    ? 'ต้องลงเวลาเข้าก่อน — ไม่ต้องรอถึง 18:00 จึงจะลงออกได้'
    : hasCheckOut
      ? 'ลงเวลาออกแล้ววันนี้'
      : 'ลงเวลาออกได้ทันทีหลังลงเข้า — ออกก่อน 18:00 จะแสดงว่าออกก่อนเวลา'

  const checkIn = async () => {
    setActing(true)
    try {
      const { data } = await api.post<AttendanceRecord>('/api/attendance/check-in', { workDate: todayStr() })
      const record = normalizeAttendance(data)!
      setToday(record)
      toast.success(record.isLateCheckIn ? `ลงเวลาเข้าแล้ว (สาย ${record.lateMinutes} นาที)` : 'ลงเวลาเข้าแล้ว — ตรงเวลา')
      refresh()
    } catch (err) {
      toast.error(formatApiError(err, 'ลงเวลาเข้าไม่สำเร็จ'))
    } finally {
      setActing(false)
    }
  }

  const checkOut = async () => {
    setActing(true)
    try {
      const { data } = await api.post<AttendanceRecord>('/api/attendance/check-out', {
        workDate: todayStr(),
        workSummary: workSummary.trim() || null,
      })
      const record = normalizeAttendance(data)!
      setToday(record)
      setWorkSummary('')
      const earlyNote = record.isEarlyCheckOut ? ' (ออกก่อน 18:00)' : ''
      toast.success(`ลงเวลาออกแล้ว${earlyNote}`)
      refresh()
    } catch (err) {
      toast.error(formatApiError(err, 'ลงเวลาออกไม่สำเร็จ'))
    } finally {
      setActing(false)
    }
  }

  return (
    <PageShell>
      <PageHeader
        icon={<Clock className="size-7" />}
        title="ลงเวลาเข้า-ออก"
        description={`เวลางาน ${SCHEDULE_IN} – ${SCHEDULE_OUT} (เวลาไทย) · เข้าหลัง ${SCHEDULE_IN} ถือว่าสาย`}
      />

      <Card className={`${premiumCardClass} overflow-visible`}>
        <div className="h-1.5 bg-gradient-to-r from-blue-500 via-indigo-500 to-violet-500 rounded-t-[inherit]" />
        <CardHeader className="border-b border-border/40 pb-4">
          <div className="flex flex-wrap items-start justify-between gap-3">
            <div>
              <CardTitle className="text-lg font-bold text-primary">วันนี้ ({todayStr()})</CardTitle>
              <CardDescription className="mt-1">
                เข้า {SCHEDULE_IN} · ออก {SCHEDULE_OUT}
              </CardDescription>
            </div>
            <Badge variant={statusBadgeVariant(today)} className="text-xs font-semibold px-3 py-1">
              {today?.statusLabel ?? 'ยังไม่ลงเวลาเข้า'}
            </Badge>
          </div>
        </CardHeader>
        <CardContent className="space-y-4 pt-5">
          <div className="grid gap-3 sm:grid-cols-2 text-sm">
            <div className="rounded-xl border border-blue-500/15 bg-blue-500/5 p-4">
              <p className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เวลาเข้า</p>
              <p className="text-2xl font-black mt-1">
                {today?.checkInLocal ?? '—'}
                {today?.isLateCheckIn && (
                  <span className="ml-2 text-destructive text-sm font-normal">
                    สาย {today.lateMinutes} นาที
                  </span>
                )}
              </p>
            </div>
            <div className="rounded-xl border border-indigo-500/15 bg-indigo-500/5 p-4">
              <p className="text-xs font-bold uppercase tracking-wider text-muted-foreground">เวลาออก</p>
              <p className="text-2xl font-black mt-1">
                {today?.checkOutLocal ?? '—'}
                {today?.isEarlyCheckOut && (
                  <span className="ml-2 text-destructive text-sm font-normal">ก่อน {SCHEDULE_OUT}</span>
                )}
              </p>
            </div>
          </div>

          {hasCheckIn && today?.workSummary && (
            <div className="rounded-lg bg-muted/50 p-3 text-sm">
              <p className="font-medium text-muted-foreground mb-1">งานวันนี้</p>
              <p className="whitespace-pre-wrap">{today.workSummary}</p>
            </div>
          )}

          {!hasCheckOut && hasCheckIn && (
            <div className="space-y-2">
              <Label htmlFor="work-summary">วันนี้ทำอะไร (ไม่บังคับ)</Label>
              <Textarea
                id="work-summary"
                placeholder="สรุปงานที่ทำวันนี้ — กรอกตอนลงเวลาออกได้"
                value={workSummary}
                onChange={(e) => setWorkSummary(e.target.value)}
                rows={3}
                maxLength={2000}
              />
            </div>
          )}

          <div className="space-y-3">
            <div className="flex flex-wrap gap-3">
              <Button
                onClick={checkIn}
                disabled={acting || !canCheckIn}
                className="rounded-xl font-bold shadow-md shadow-primary/20 btn-premium min-w-[140px]"
              >
                <LogIn className="size-4 mr-1.5" />
                {canCheckIn ? 'ลงเวลาเข้า' : 'ลงเวลาเข้าแล้ว'}
              </Button>
              <Button
                variant="outline"
                onClick={checkOut}
                disabled={acting || !canCheckOut}
                className="rounded-xl font-semibold min-w-[140px]"
              >
                <LogOut className="size-4 mr-1.5" />
                {hasCheckOut ? 'ลงเวลาออกแล้ว' : 'ลงเวลาออก'}
              </Button>
            </div>
            <p className="text-xs text-muted-foreground">{checkInHint}</p>
            <p className="text-xs text-muted-foreground">{checkOutHint}</p>
          </div>
        </CardContent>
      </Card>

      <div className={filterBarClass}>
        <div className="space-y-2">
          <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">จาก</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} className="rounded-xl w-[160px]" />
        </div>
        <div className="space-y-2">
          <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">ถึง</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} className="rounded-xl w-[160px]" />
        </div>
      </div>

      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
            <History className="size-5" />
            ประวัติ{isApprover ? '' : ` — ${me?.fullName}`}
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow className="bg-muted/30 hover:bg-muted/30">
                {isApprover && <TableHead className="font-bold">พนักงาน</TableHead>}
                <TableHead className="font-bold">วันที่</TableHead>
                <TableHead>เข้า</TableHead>
                <TableHead>ออก</TableHead>
                <TableHead>สถานะ</TableHead>
                <TableHead>งานวันนี้</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((a) => (
                <TableRow key={a.id} className="hover:bg-blue-500/[0.02]">
                  {isApprover && <TableCell>{a.employeeName}</TableCell>}
                  <TableCell>{a.workDate}</TableCell>
                  <TableCell>
                    {a.checkInLocal ?? '—'}
                    {a.isLateCheckIn && (
                      <span className="block text-xs text-destructive">สาย {a.lateMinutes} นาที</span>
                    )}
                  </TableCell>
                  <TableCell>
                    {a.checkOutLocal ?? '—'}
                    {a.isEarlyCheckOut && (
                      <span className="block text-xs text-destructive">ก่อนเวลา</span>
                    )}
                  </TableCell>
                  <TableCell>
                    <Badge variant={statusBadgeVariant(a)} className="font-normal">
                      {a.statusLabel}
                    </Badge>
                  </TableCell>
                  <TableCell className="max-w-[240px] truncate text-muted-foreground">
                    {a.workSummary ?? '—'}
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={isApprover ? 6 : 5} className="text-center text-muted-foreground">
                    ไม่มีข้อมูล
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </PageShell>
  )
}
