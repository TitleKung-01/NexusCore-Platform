import { useEffect, useMemo, useState } from 'react'
import { api, formatApiError } from '@/api'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
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

  const year = useMemo(() => (from ? Number(from.slice(0, 4)) : new Date().getFullYear()), [from])

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
    const params: Record<string, string> = { from, to }
    if (departmentId) params.departmentId = departmentId
    api
      .get<LeaveCalendarEntry[]>('/api/leave-requests/calendar', { params })
      .then((r) => setEntries(r.data))
      .catch((e) => {
        setEntries([])
        setError(formatApiError(e, 'โหลดปฏิทินไม่สำเร็จ'))
      })
    api
      .get<Holiday[]>('/api/holidays', { params: { year } })
      .then((r) => setHolidays(r.data))
      .catch(() => setHolidays([]))
  }, [from, to, departmentId, year])

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">ปฏิทินลา</h2>
      <div className="flex flex-wrap gap-4 items-end">
        <div className="space-y-2">
          <Label>จากวันที่</Label>
          <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} />
        </div>
        <div className="space-y-2">
          <Label>ถึงวันที่</Label>
          <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} />
        </div>
        {isHr && (
          <div className="space-y-2 min-w-[200px]">
            <Label>แผนก</Label>
            <Select value={departmentId || 'all'} onValueChange={(v) => setDepartmentId(v === 'all' ? '' : v)}>
              <SelectTrigger>
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
      </div>
      {error && <p className="text-destructive text-sm">{error}</p>}
      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>วันลาในช่วงที่เลือก</CardTitle>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>พนักงาน</TableHead>
                  <TableHead>ประเภท</TableHead>
                  <TableHead>ช่วงวัน</TableHead>
                  <TableHead>สถานะ</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {entries.map((e) => (
                  <TableRow key={e.id}>
                    <TableCell>{e.employeeName}</TableCell>
                    <TableCell>{e.leaveTypeName}</TableCell>
                    <TableCell>
                      {e.startDate} — {e.endDate}
                    </TableCell>
                    <TableCell>
                      <WorkflowStatusBadge status={e.status} />
                    </TableCell>
                  </TableRow>
                ))}
                {entries.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={4} className="text-center text-muted-foreground">
                      ไม่มีรายการลา
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>วันหยุดประจำปี {year}</CardTitle>
          </CardHeader>
          <CardContent>
            <ul className="space-y-2 text-sm">
              {holidays.map((h) => (
                <li key={h.id} className="flex justify-between border-b pb-1">
                  <span>{h.name}</span>
                  <span className="text-muted-foreground">{h.date}</span>
                </li>
              ))}
              {holidays.length === 0 && <p className="text-muted-foreground">ไม่มีวันหยุด</p>}
            </ul>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
