import { useEffect, useState } from 'react'
import { CalendarRange, ClipboardList, Clock, Download, FileSpreadsheet } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'
import { PageHeader, PageShell, premiumCardClass } from '@/features/shared/PageHeader'
import type { EmployeeListItem } from '@/types/api'

function todayBangkok() {
  return new Date().toLocaleDateString('en-CA', { timeZone: 'Asia/Bangkok' })
}

function monthStartBangkok() {
  const today = todayBangkok()
  return today.slice(0, 8) + '01'
}

function downloadBlob(data: Blob, filename: string) {
  const url = URL.createObjectURL(data)
  const a = document.createElement('a')
  a.href = url
  a.download = filename
  a.click()
  URL.revokeObjectURL(url)
}

export function ReportsPage() {
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [downloadingLeave, setDownloadingLeave] = useState(false)

  const [from, setFrom] = useState(monthStartBangkok)
  const [to, setTo] = useState(todayBangkok)
  const [employeeId, setEmployeeId] = useState<string>('all')
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [downloadingAttendance, setDownloadingAttendance] = useState(false)

  useEffect(() => {
    api
      .get<EmployeeListItem[]>('/api/employees')
      .then((r) => setEmployees(r.data))
      .catch(() => setEmployees([]))
  }, [])

  const downloadLeave = async () => {
    setDownloadingLeave(true)
    try {
      const { data } = await api.get<Blob>('/api/reports/leave-summary', {
        params: { year },
        responseType: 'blob',
      })
      downloadBlob(data, `leave-summary-${year}.csv`)
      toast.success('ดาวน์โหลดแล้ว')
    } catch (err) {
      toast.error(formatApiError(err, 'ดาวน์โหลดไม่สำเร็จ'))
    } finally {
      setDownloadingLeave(false)
    }
  }

  const downloadAttendance = async () => {
    if (!from || !to) {
      toast.error('เลือกช่วงวันที่')
      return
    }
    if (from > to) {
      toast.error('วันที่เริ่มต้องไม่เกินวันที่สิ้นสุด')
      return
    }

    setDownloadingAttendance(true)
    try {
      const params: Record<string, string> = { from, to }
      if (employeeId !== 'all') params.employeeId = employeeId

      const { data } = await api.get<Blob>('/api/reports/attendance', {
        params,
        responseType: 'blob',
      })
      const suffix = employeeId === 'all' ? 'all' : employeeId.slice(0, 8)
      downloadBlob(data, `attendance-${from}-to-${to}-${suffix}.csv`)
      toast.success('ดาวน์โหลดแล้ว')
    } catch (err) {
      toast.error(formatApiError(err, 'ดาวน์โหลดไม่สำเร็จ'))
    } finally {
      setDownloadingAttendance(false)
    }
  }

  return (
    <PageShell>
      <PageHeader
        icon={<ClipboardList className="size-7" />}
        title="รายงาน HR"
        description="ส่งออกข้อมูลลงเวลาและสรุปวันลาเป็นไฟล์ CSV สำหรับ Excel"
      />

      <div className="grid gap-6 lg:grid-cols-2">
        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <div className="flex items-start gap-3">
              <div className="size-11 rounded-xl bg-blue-500/10 flex items-center justify-center shrink-0">
                <Clock className="size-5 text-primary" />
              </div>
              <div>
                <CardTitle className="text-lg font-bold">ลงเวลาเข้า-ออก (CSV)</CardTitle>
                <CardDescription className="mt-1">
                  ส่งออกข้อมูลลงเวลาพนักงานตามช่วงวันที่ — เปิดใน Excel ได้ (UTF-8)
                </CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-4 pt-5">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-2">
                <Label className="font-semibold">จากวันที่</Label>
                <Input type="date" value={from} onChange={(e) => setFrom(e.target.value)} required className="rounded-xl" />
              </div>
              <div className="space-y-2">
                <Label className="font-semibold">ถึงวันที่</Label>
                <Input type="date" value={to} onChange={(e) => setTo(e.target.value)} required className="rounded-xl" />
              </div>
            </div>
            <div className="space-y-2">
              <Label className="font-semibold">พนักงาน</Label>
              <Select value={employeeId} onValueChange={setEmployeeId}>
                <SelectTrigger className="rounded-xl">
                  <SelectValue placeholder="ทุกคน" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">ทุกคน</SelectItem>
                  {employees.map((e) => (
                    <SelectItem key={e.userId} value={e.userId}>
                      {e.fullName} ({e.username})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <Button
              onClick={downloadAttendance}
              disabled={downloadingAttendance}
              className="w-full rounded-xl font-bold shadow-md shadow-primary/20 btn-premium"
            >
              <Download className="size-4 mr-1.5" />
              {downloadingAttendance ? 'กำลังดาวน์โหลด...' : 'ดาวน์โหลด CSV ลงเวลา'}
            </Button>
          </CardContent>
        </Card>

        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <div className="flex items-start gap-3">
              <div className="size-11 rounded-xl bg-emerald-500/10 flex items-center justify-center shrink-0">
                <CalendarRange className="size-5 text-emerald-600" />
              </div>
              <div>
                <CardTitle className="text-lg font-bold">สรุปวันลาประจำปี (CSV)</CardTitle>
                <CardDescription className="mt-1">
                  รายงานการใช้สิทธิ์ลาของพนักงานทั้งปี
                </CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-4 pt-5">
            <div className="space-y-2">
              <Label className="font-semibold">ปี</Label>
              <Input
                type="number"
                value={year}
                onChange={(e) => setYear(e.target.value)}
                min={2020}
                max={2100}
                className="rounded-xl max-w-[140px]"
              />
            </div>
            <div className="rounded-xl border border-dashed border-border/60 bg-muted/30 p-4 flex items-center gap-3 text-sm text-muted-foreground">
              <FileSpreadsheet className="size-8 shrink-0 text-primary/60" />
              <p>ไฟล์ CSV รองรับภาษาไทยและเปิดใน Microsoft Excel หรือ Google Sheets ได้ทันที</p>
            </div>
            <Button
              onClick={downloadLeave}
              disabled={downloadingLeave}
              variant="outline"
              className="w-full rounded-xl font-semibold"
            >
              <Download className="size-4 mr-1.5" />
              {downloadingLeave ? 'กำลังดาวน์โหลด...' : 'ดาวน์โหลด CSV วันลา'}
            </Button>
          </CardContent>
        </Card>
      </div>
    </PageShell>
  )
}
