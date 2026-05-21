import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useAuth } from '@/features/auth/AuthContext'
import type { AttendanceRecord } from '@/types/api'

function todayStr() {
  return new Date().toISOString().slice(0, 10)
}

function weekRange() {
  const to = todayStr()
  const d = new Date()
  d.setDate(d.getDate() - 6)
  const from = d.toISOString().slice(0, 10)
  return { from, to }
}

export function AttendancePage() {
  const { me, isApprover } = useAuth()
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [items, setItems] = useState<AttendanceRecord[]>([])
  const [acting, setActing] = useState(false)

  const load = () => {
    if (!from || !to) return
    api
      .get<AttendanceRecord[]>('/api/attendance', { params: { from, to } })
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(() => {
    const { from: f, to: t } = weekRange()
    setFrom(f)
    setTo(t)
  }, [])

  useEffect(load, [from, to])

  const checkIn = async () => {
    setActing(true)
    try {
      await api.post('/api/attendance/check-in', { workDate: todayStr() })
      toast.success('ลงเวลาเข้าแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ลงเวลาเข้าไม่สำเร็จ'))
    } finally {
      setActing(false)
    }
  }

  const checkOut = async () => {
    setActing(true)
    try {
      await api.post('/api/attendance/check-out', { workDate: todayStr() })
      toast.success('ลงเวลาออกแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ลงเวลาออกไม่สำเร็จ'))
    } finally {
      setActing(false)
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">ลงเวลาเข้า-ออก</h2>
      <Card>
        <CardHeader>
          <CardTitle>วันนี้ ({todayStr()})</CardTitle>
        </CardHeader>
        <CardContent className="flex gap-3">
          <Button onClick={checkIn} disabled={acting}>
            ลงเวลาเข้า
          </Button>
          <Button variant="outline" onClick={checkOut} disabled={acting}>
            ลงเวลาออก
          </Button>
        </CardContent>
      </Card>
      <div className="flex flex-wrap gap-4 items-end">
        <div className="space-y-2">
          <Label>จาก</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
        </div>
        <div className="space-y-2">
          <Label>ถึง</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} />
        </div>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>
            ประวัติ{isApprover ? '' : ` — ${me?.fullName}`}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                {isApprover && <TableHead>พนักงาน</TableHead>}
                <TableHead>วันที่</TableHead>
                <TableHead>เข้า</TableHead>
                <TableHead>ออก</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((a) => (
                <TableRow key={a.id}>
                  {isApprover && <TableCell>{a.employeeName}</TableCell>}
                  <TableCell>{a.workDate}</TableCell>
                  <TableCell>{a.checkInUtc ? new Date(a.checkInUtc).toLocaleString('th-TH') : '—'}</TableCell>
                  <TableCell>{a.checkOutUtc ? new Date(a.checkOutUtc).toLocaleString('th-TH') : '—'}</TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={isApprover ? 4 : 3} className="text-center text-muted-foreground">
                    ไม่มีข้อมูล
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  )
}
