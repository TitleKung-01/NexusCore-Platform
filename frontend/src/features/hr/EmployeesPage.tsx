import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import type { Department, EmployeeListItem, EmployeeTransfer } from '@/types/api'

const ROLES = ['Employee', 'Manager', 'Hr', 'Admin']

export function EmployeesPage() {
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [departments, setDepartments] = useState<Department[]>([])
  const [transfers, setTransfers] = useState<EmployeeTransfer[]>([])
  const [editOpen, setEditOpen] = useState(false)
  const [selected, setSelected] = useState<EmployeeListItem | null>(null)
  const [departmentId, setDepartmentId] = useState('')
  const [role, setRole] = useState('')
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [saving, setSaving] = useState(false)

  const load = () => {
    api.get<EmployeeListItem[]>('/api/employees').then((r) => setEmployees(r.data)).catch(() => setEmployees([]))
    api.get<EmployeeTransfer[]>('/api/employee-transfers').then((r) => setTransfers(r.data.slice(0, 20))).catch(() => setTransfers([]))
  }

  useEffect(() => {
    load()
    api.get<Department[]>('/api/departments').then((r) => setDepartments(r.data)).catch(() => setDepartments([]))
  }, [])

  const openEdit = (emp: EmployeeListItem) => {
    setSelected(emp)
    setFullName(emp.fullName)
    setEmail(emp.email)
    setRole(emp.role)
    const dept = departments.find((d) => d.name === emp.departmentName)
    setDepartmentId(dept?.id ?? '')
    setEditOpen(true)
  }

  const save = async () => {
    if (!selected) return
    setSaving(true)
    try {
      await api.put(`/api/employees/${selected.userId}`, {
        departmentId: departmentId || null,
        role,
        fullName,
        email,
      })
      toast.success('บันทึกแล้ว')
      setEditOpen(false)
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">พนักงาน</h2>
      <Card>
        <CardHeader>
          <CardTitle>รายชื่อพนักงาน</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ชื่อ</TableHead>
                <TableHead>Username</TableHead>
                <TableHead>แผนก</TableHead>
                <TableHead>บทบาท</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {employees.map((e) => (
                <TableRow key={e.userId}>
                  <TableCell>{e.fullName}</TableCell>
                  <TableCell>{e.username}</TableCell>
                  <TableCell>{e.departmentName}</TableCell>
                  <TableCell>{e.role}</TableCell>
                  <TableCell>
                    <Button size="sm" variant="outline" onClick={() => openEdit(e)}>
                      แก้ไข
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <CardTitle>ประวัติโยกย้ายแผนกล่าสุด</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>พนักงาน</TableHead>
                <TableHead>จาก</TableHead>
                <TableHead>ไป</TableHead>
                <TableHead>มีผล</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {transfers.map((t) => (
                <TableRow key={t.id}>
                  <TableCell>{t.employeeName}</TableCell>
                  <TableCell>{t.fromDepartment}</TableCell>
                  <TableCell>{t.toDepartment}</TableCell>
                  <TableCell>{t.effectiveDate}</TableCell>
                </TableRow>
              ))}
              {transfers.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} className="text-center text-muted-foreground">
                    ยังไม่มีประวัติ
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
      <Dialog open={editOpen} onOpenChange={setEditOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>แก้ไขพนักงาน</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-2">
              <Label>ชื่อ-นามสกุล</Label>
              <Input value={fullName} onChange={(e) => setFullName(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>อีเมล</Label>
              <Input value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>แผนก</Label>
              <Select value={departmentId} onValueChange={setDepartmentId}>
                <SelectTrigger>
                  <SelectValue placeholder="เลือกแผนก" />
                </SelectTrigger>
                <SelectContent>
                  {departments.map((d) => (
                    <SelectItem key={d.id} value={d.id}>
                      {d.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>บทบาท</Label>
              <Select value={role} onValueChange={setRole}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {ROLES.map((r) => (
                    <SelectItem key={r} value={r}>
                      {r}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setEditOpen(false)}>
              ยกเลิก
            </Button>
            <Button onClick={save} disabled={saving}>
              {saving ? 'กำลังบันทึก...' : 'บันทึก'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
