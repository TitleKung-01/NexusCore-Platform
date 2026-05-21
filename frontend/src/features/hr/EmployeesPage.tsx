import { useEffect, useState } from 'react'
import { Building2, Plus, Shield, Users, ArrowRightLeft } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
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
import { useAuth } from '@/features/auth/AuthContext'
import { PageHeader, PageShell, premiumCardClass } from '@/features/shared/PageHeader'
import type { Department, EmployeeListItem, EmployeeTransfer, RoleDefinition } from '@/types/api'

export function EmployeesPage() {
  const { isHr } = useAuth()
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [departments, setDepartments] = useState<Department[]>([])
  const [roles, setRoles] = useState<RoleDefinition[]>([])
  const [transfers, setTransfers] = useState<EmployeeTransfer[]>([])
  const [newDeptName, setNewDeptName] = useState('')
  const [newDeptCode, setNewDeptCode] = useState('')
  const [newRoleName, setNewRoleName] = useState('')
  const [newRoleDesc, setNewRoleDesc] = useState('')
  const [newRoleCanApprove, setNewRoleCanApprove] = useState(false)
  const [newRoleHrAccess, setNewRoleHrAccess] = useState(false)
  const [creatingDept, setCreatingDept] = useState(false)
  const [creatingRole, setCreatingRole] = useState(false)
  const [editOpen, setEditOpen] = useState(false)
  const [selected, setSelected] = useState<EmployeeListItem | null>(null)
  const [departmentId, setDepartmentId] = useState('')
  const [role, setRole] = useState('')
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [saving, setSaving] = useState(false)

  const load = () => {
    api.get<EmployeeListItem[]>('/api/employees').then((r) => setEmployees(r.data)).catch(() => setEmployees([]))
    api
      .get<EmployeeTransfer[]>('/api/employee-transfers', { params: { limit: 20 } })
      .then((r) => setTransfers(r.data))
      .catch(() => setTransfers([]))
  }

  const loadDepartments = () => {
    api.get<Department[]>('/api/departments').then((r) => setDepartments(r.data)).catch(() => setDepartments([]))
  }

  const loadRoles = () => {
    api.get<RoleDefinition[]>('/api/roles').then((r) => setRoles(r.data)).catch(() => setRoles([]))
  }

  useEffect(() => {
    load()
    loadDepartments()
    loadRoles()
  }, [])

  const createDepartment = async (e: React.FormEvent) => {
    e.preventDefault()
    setCreatingDept(true)
    try {
      await api.post('/api/departments', {
        name: newDeptName.trim(),
        code: newDeptCode.trim().toUpperCase(),
      })
      toast.success('สร้างแผนกแล้ว')
      setNewDeptName('')
      setNewDeptCode('')
      loadDepartments()
    } catch (err) {
      toast.error(formatApiError(err, 'สร้างแผนกไม่สำเร็จ'))
    } finally {
      setCreatingDept(false)
    }
  }

  const createRole = async (e: React.FormEvent) => {
    e.preventDefault()
    setCreatingRole(true)
    try {
      await api.post('/api/roles', {
        name: newRoleName.trim(),
        description: newRoleDesc.trim() || null,
        canApprove: newRoleCanApprove,
        isHrAccess: newRoleHrAccess,
      })
      toast.success('สร้างบทบาทแล้ว')
      setNewRoleName('')
      setNewRoleDesc('')
      setNewRoleCanApprove(false)
      setNewRoleHrAccess(false)
      loadRoles()
    } catch (err) {
      toast.error(formatApiError(err, 'สร้างบทบาทไม่สำเร็จ'))
    } finally {
      setCreatingRole(false)
    }
  }

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
    <PageShell>
      <PageHeader
        icon={<Users className="size-7" />}
        title="จัดการพนักงาน"
        description="ดูรายชื่อพนักงาน แผนก บทบาท และประวัติการโยกย้ายแผนก"
      />

      <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
        <div className="rounded-xl border border-blue-500/10 bg-blue-500/5 p-4">
          <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">พนักงานทั้งหมด</p>
          <p className="text-2xl font-black mt-0.5">{employees.length}</p>
        </div>
        <div className="rounded-xl border border-indigo-500/10 bg-indigo-500/5 p-4">
          <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">แผนก</p>
          <p className="text-2xl font-black mt-0.5">{departments.length}</p>
        </div>
        <div className="rounded-xl border border-violet-500/10 bg-violet-500/5 p-4 col-span-2 sm:col-span-1">
          <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">บทบาท</p>
          <p className="text-2xl font-black mt-0.5">{roles.length}</p>
        </div>
      </div>

      {isHr && (
        <div className="grid gap-4 lg:grid-cols-2">
          <Card className={premiumCardClass}>
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <Building2 className="size-5" />
                สร้างแผนกใหม่
              </CardTitle>
              <CardDescription>รหัสแผนกไม่ซ้ำ (เช่น MKT, FIN)</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={createDepartment} className="space-y-3">
                <div className="space-y-2">
                  <Label>ชื่อแผนก</Label>
                  <Input value={newDeptName} onChange={(e) => setNewDeptName(e.target.value)} required />
                </div>
                <div className="space-y-2">
                  <Label>รหัสแผนก</Label>
                  <Input
                    value={newDeptCode}
                    onChange={(e) => setNewDeptCode(e.target.value.toUpperCase())}
                    placeholder="เช่น MKT"
                    maxLength={16}
                    required
                  />
                </div>
                <Button type="submit" disabled={creatingDept} className="rounded-xl btn-premium">
                  <Plus className="size-4 mr-1" />
                  {creatingDept ? 'กำลังสร้าง...' : 'สร้างแผนก'}
                </Button>
              </form>
            </CardContent>
          </Card>
          <Card className={premiumCardClass}>
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <Shield className="size-5" />
                สร้างบทบาทใหม่
              </CardTitle>
              <CardDescription>กำหนดสิทธิอนุมัติและเมนู HR</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={createRole} className="space-y-3">
                <div className="space-y-2">
                  <Label>ชื่อบทบาท</Label>
                  <Input value={newRoleName} onChange={(e) => setNewRoleName(e.target.value)} required />
                </div>
                <div className="space-y-2">
                  <Label>คำอธิบาย (ไม่บังคับ)</Label>
                  <Input value={newRoleDesc} onChange={(e) => setNewRoleDesc(e.target.value)} />
                </div>
                <label className="flex items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    checked={newRoleCanApprove}
                    onChange={(e) => setNewRoleCanApprove(e.target.checked)}
                  />
                  อนุมัติคำขอ (ลา / OT / เบิก)
                </label>
                <label className="flex items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    checked={newRoleHrAccess}
                    onChange={(e) => setNewRoleHrAccess(e.target.checked)}
                  />
                  เข้าเมนู HR (พนักงาน, รายงาน, วันหยุด)
                </label>
                <Button type="submit" disabled={creatingRole} className="rounded-xl btn-premium">
                  <Plus className="size-4 mr-1" />
                  {creatingRole ? 'กำลังสร้าง...' : 'สร้างบทบาท'}
                </Button>
              </form>
            </CardContent>
          </Card>
        </div>
      )}

      {isHr && (
        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <Shield className="size-5" />
              บทบาทในระบบ
            </CardTitle>
          </CardHeader>
          <CardContent className="pt-4">
            {roles.length === 0 ? (
              <p className="text-sm text-muted-foreground">ยังไม่มีบทบาท — รีสตาร์ท API หรือรอ seed ข้อมูล</p>
            ) : (
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>ชื่อ</TableHead>
                    <TableHead>อนุมัติ</TableHead>
                    <TableHead>เมนู HR</TableHead>
                    <TableHead>ระบบ</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {roles.map((r) => (
                    <TableRow key={r.id}>
                      <TableCell>
                        {r.name}
                        {r.description && (
                          <span className="block text-xs text-muted-foreground">{r.description}</span>
                        )}
                      </TableCell>
                      <TableCell>{r.canApprove ? 'ใช่' : '—'}</TableCell>
                      <TableCell>{r.isHrAccess ? 'ใช่' : '—'}</TableCell>
                      <TableCell>{r.isBuiltIn ? 'ค่าเริ่มต้น' : 'กำหนดเอง'}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </CardContent>
        </Card>
      )}

      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
            <Users className="size-5" />
            รายชื่อพนักงาน
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0 pt-0">
          <Table>
            <TableHeader>
              <TableRow className="bg-muted/30 hover:bg-muted/30">
                <TableHead className="font-bold">ชื่อ</TableHead>
                <TableHead>Username</TableHead>
                <TableHead>แผนก</TableHead>
                <TableHead>บทบาท</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {employees.map((e) => (
                <TableRow key={e.userId} className="hover:bg-blue-500/[0.02]">
                  <TableCell className="font-medium">{e.fullName}</TableCell>
                  <TableCell className="text-muted-foreground">{e.username}</TableCell>
                  <TableCell>{e.departmentName}</TableCell>
                  <TableCell>
                    <span className="inline-flex rounded-lg bg-primary/10 text-primary px-2 py-0.5 text-xs font-semibold">
                      {e.role}
                    </span>
                  </TableCell>
                  <TableCell>
                    <Button size="sm" variant="outline" className="rounded-lg" onClick={() => openEdit(e)}>
                      แก้ไข
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
            <ArrowRightLeft className="size-5" />
            ประวัติโยกย้ายแผนกล่าสุด
          </CardTitle>
          <CardDescription>บันทึกอัตโนมัติเมื่อ HR แก้ไขแผนกของพนักงาน (20 รายการล่าสุด)</CardDescription>
        </CardHeader>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow className="bg-muted/30 hover:bg-muted/30">
                <TableHead className="font-bold">พนักงาน</TableHead>
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
                  {roles.map((r) => (
                    <SelectItem key={r.id} value={r.name}>
                      {r.name}
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
    </PageShell>
  )
}
