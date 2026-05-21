import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError, getStoredToken } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { useAuth } from '@/features/auth/AuthContext'
import type { EmployeeListItem, Payslip } from '@/types/api'

export function PayslipsPage() {
  const { isHr, me } = useAuth()
  const [items, setItems] = useState<Payslip[]>([])
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [employeeId, setEmployeeId] = useState('')
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [month, setMonth] = useState(String(new Date().getMonth() + 1))
  const [file, setFile] = useState<File | null>(null)
  const [uploading, setUploading] = useState(false)

  const load = () => {
    api
      .get<Payslip[]>('/api/payslips')
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(() => {
    load()
    if (isHr) {
      api.get<EmployeeListItem[]>('/api/employees').then((r) => setEmployees(r.data)).catch(() => setEmployees([]))
    }
  }, [isHr])

  const download = async (id: string, fileName: string) => {
    try {
      const token = getStoredToken()
      const base = import.meta.env.VITE_API_URL ?? ''
      const res = await fetch(`${base}/api/payslips/${id}/download`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      })
      if (!res.ok) throw new Error('download failed')
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = fileName
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      toast.error('ดาวน์โหลดไม่สำเร็จ')
    }
  }

  const upload = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!file || !employeeId) return
    setUploading(true)
    try {
      const form = new FormData()
      form.append('employeeId', employeeId)
      form.append('year', year)
      form.append('month', month)
      form.append('file', file)
      await api.post('/api/payslips', form)
      toast.success('อัปโหลดแล้ว')
      setFile(null)
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'อัปโหลดไม่สำเร็จ'))
    } finally {
      setUploading(false)
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">สลิปเงินเดือน</h2>
      {isHr && (
        <Card>
          <CardHeader>
            <CardTitle>อัปโหลดสลิป (HR)</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={upload} className="space-y-4">
              <div className="space-y-2">
                <Label>พนักงาน</Label>
                <Select value={employeeId} onValueChange={setEmployeeId}>
                  <SelectTrigger>
                    <SelectValue placeholder="เลือกพนักงาน" />
                  </SelectTrigger>
                  <SelectContent>
                    {employees.map((e) => (
                      <SelectItem key={e.userId} value={e.userId}>
                        {e.fullName}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label>ปี</Label>
                  <Input type="number" value={year} onChange={(e) => setYear(e.target.value)} />
                </div>
                <div className="space-y-2">
                  <Label>เดือน</Label>
                  <Input type="number" min={1} max={12} value={month} onChange={(e) => setMonth(e.target.value)} />
                </div>
              </div>
              <div className="space-y-2">
                <Label>ไฟล์ PDF</Label>
                <Input type="file" accept=".pdf" onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
              </div>
              <Button type="submit" disabled={uploading || !file}>
                {uploading ? 'กำลังอัปโหลด...' : 'อัปโหลด'}
              </Button>
            </form>
          </CardContent>
        </Card>
      )}
      <Card>
        <CardHeader>
          <CardTitle>{isHr ? 'สลิปทั้งหมด' : `สลิปของ ${me?.fullName}`}</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ปี/เดือน</TableHead>
                <TableHead>ไฟล์</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>
                    {p.year}/{p.month}
                  </TableCell>
                  <TableCell>{p.fileName}</TableCell>
                  <TableCell>
                    <Button size="sm" variant="outline" onClick={() => download(p.id, p.fileName)}>
                      ดาวน์โหลด
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={3} className="text-center text-muted-foreground">
                    ยังไม่มีสลิป
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
