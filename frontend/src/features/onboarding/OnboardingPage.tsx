import { useCallback, useEffect, useMemo, useState } from 'react'
import { CheckCircle2, ClipboardList, ListChecks, Plus, Search, UserCheck } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Badge } from '@/components/ui/badge'
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
import { Separator } from '@/components/ui/separator'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { cn } from '@/lib/utils'
import { useAuth } from '@/features/auth/AuthContext'
import { PageHeader, PageShell, premiumCardClass } from '@/features/shared/PageHeader'
import type { EmployeeListItem, EmployeeOnboardingTask, OnboardingTemplate } from '@/types/api'

type EditTaskRow = { id?: string; title: string }

export function OnboardingPage() {
  const { isHr, me } = useAuth()
  const [tasks, setTasks] = useState<EmployeeOnboardingTask[]>([])
  const [templates, setTemplates] = useState<OnboardingTemplate[]>([])
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [templateId, setTemplateId] = useState('')
  const [employeeId, setEmployeeId] = useState('')
  const [employeeSearch, setEmployeeSearch] = useState('')

  const [editorOpen, setEditorOpen] = useState(false)
  const [editingTemplateId, setEditingTemplateId] = useState<string | null>(null)
  const [templateName, setTemplateName] = useState('')
  const [editTasks, setEditTasks] = useState<EditTaskRow[]>([{ title: '' }])
  const [saving, setSaving] = useState(false)

  const loadTasks = () => {
    api
      .get<EmployeeOnboardingTask[]>('/api/onboarding/tasks')
      .then((r) => setTasks(r.data.sort((a, b) => a.sortOrder - b.sortOrder)))
      .catch(() => setTasks([]))
  }

  const loadTemplates = useCallback(() => {
    api
      .get<OnboardingTemplate[]>('/api/onboarding/templates')
      .then((r) => setTemplates(r.data))
      .catch(() => setTemplates([]))
  }, [])

  useEffect(() => {
    loadTasks()
    if (isHr) {
      loadTemplates()
      api.get<EmployeeListItem[]>('/api/employees').then((r) => setEmployees(r.data)).catch(() => setEmployees([]))
    }
  }, [isHr, loadTemplates])

  const toggleTask = async (taskId: string, completed: boolean) => {
    try {
      await api.post(`/api/onboarding/tasks/${taskId}/complete`, { isCompleted: completed })
      loadTasks()
    } catch (err) {
      toast.error(formatApiError(err, 'อัปเดตไม่สำเร็จ'))
    }
  }

  const assign = async () => {
    if (!templateId || !employeeId) return
    try {
      await api.post('/api/onboarding/assign', { templateId, employeeId })
      toast.success('มอบหมายแล้ว')
      loadTasks()
    } catch (err) {
      toast.error(formatApiError(err, 'มอบหมายไม่สำเร็จ'))
    }
  }

  const openCreateEditor = () => {
    setEditingTemplateId(null)
    setTemplateName('')
    setEditTasks([{ title: '' }])
    setEditorOpen(true)
  }

  const openEditEditor = (template: OnboardingTemplate) => {
    setEditingTemplateId(template.id)
    setTemplateName(template.name)
    setEditTasks(
      template.tasks.length > 0
        ? template.tasks.map((t) => ({ id: t.id, title: t.title }))
        : [{ title: '' }]
    )
    setEditorOpen(true)
  }

  const addTaskRow = () => setEditTasks((rows) => [...rows, { title: '' }])

  const removeTaskRow = (index: number) =>
    setEditTasks((rows) => (rows.length <= 1 ? rows : rows.filter((_, i) => i !== index)))

  const updateTaskTitle = (index: number, title: string) =>
    setEditTasks((rows) => rows.map((row, i) => (i === index ? { ...row, title } : row)))

  const saveTemplate = async () => {
    const name = templateName.trim()
    const taskPayload = editTasks
      .map((row, index) => ({
        id: row.id ?? null,
        title: row.title.trim(),
        sortOrder: index + 1,
      }))
      .filter((t) => t.title.length > 0)

    if (!name) {
      toast.error('กรุณาระบุชื่อเทมเพลต')
      return
    }
    if (taskPayload.length === 0) {
      toast.error('กรุณาเพิ่มงานอย่างน้อย 1 รายการ')
      return
    }

    setSaving(true)
    try {
      const body = { name, tasks: taskPayload }
      if (editingTemplateId) {
        await api.put(`/api/onboarding/templates/${editingTemplateId}`, body)
        toast.success('บันทึกเทมเพลตแล้ว')
      } else {
        await api.post('/api/onboarding/templates', body)
        toast.success('สร้างเทมเพลตแล้ว')
      }
      setEditorOpen(false)
      loadTemplates()
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    } finally {
      setSaving(false)
    }
  }

  const filteredEmployees = useMemo(() => {
    const q = employeeSearch.trim().toLowerCase()
    if (!q) return employees
    return employees.filter(
      (e) =>
        e.fullName.toLowerCase().includes(q) ||
        e.username.toLowerCase().includes(q) ||
        e.email.toLowerCase().includes(q) ||
        e.departmentName.toLowerCase().includes(q)
    )
  }, [employees, employeeSearch])

  const selectedEmployee = employees.find((e) => e.userId === employeeId)
  const selectedTemplate = templates.find((t) => t.id === templateId)
  const canAssign = Boolean(templateId && employeeId)

  const myTasks = tasks.filter((t) => t.employeeId === me?.userId)
  const completedCount = myTasks.filter((t) => t.isCompleted).length
  const progressPct = myTasks.length > 0 ? Math.round((completedCount / myTasks.length) * 100) : 0

  return (
    <PageShell>
      <PageHeader
        icon={<UserCheck className="size-7" />}
        title="Onboarding"
        description="ติดตามงานต้อนรับพนักงานใหม่และมอบหมายชุดงานจากเทมเพลต"
        actions={
          isHr ? (
            <Button type="button" className="rounded-xl font-bold btn-premium" onClick={openCreateEditor}>
              <Plus className="size-4 mr-1" />
              สร้างเทมเพลตใหม่
            </Button>
          ) : undefined
        }
      />

      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <ListChecks className="size-5" />
                งานของฉัน
              </CardTitle>
              {myTasks.length > 0 && (
                <p className="text-sm text-muted-foreground mt-1">
                  ทำแล้ว {completedCount} จาก {myTasks.length} งาน ({progressPct}%)
                </p>
              )}
            </div>
            {myTasks.length > 0 && (
              <div className="flex items-center gap-2 min-w-[140px]">
                <div className="flex-1 h-2 rounded-full bg-muted overflow-hidden">
                  <div
                    className="h-full rounded-full bg-gradient-to-r from-blue-500 to-indigo-600 transition-all"
                    style={{ width: `${progressPct}%` }}
                  />
                </div>
                <span className="text-xs font-bold text-primary tabular-nums">{progressPct}%</span>
              </div>
            )}
          </div>
        </CardHeader>
        <CardContent className="space-y-2 pt-4">
          {myTasks.map((t) => (
            <label
              key={t.id}
              className={cn(
                'flex items-center gap-3 rounded-xl border p-4 cursor-pointer transition-all hover:shadow-sm',
                t.isCompleted
                  ? 'border-emerald-500/20 bg-emerald-500/5'
                  : 'border-border/60 bg-card hover:border-primary/20'
              )}
            >
              <input
                type="checkbox"
                checked={t.isCompleted}
                onChange={(e) => toggleTask(t.id, e.target.checked)}
                className="size-4 rounded accent-primary"
              />
              <span className={cn('flex-1 font-medium', t.isCompleted && 'line-through text-muted-foreground')}>
                {t.title}
              </span>
              {t.isCompleted && <CheckCircle2 className="size-4 text-emerald-600 shrink-0" />}
            </label>
          ))}
          {myTasks.length === 0 && (
            <p className="text-muted-foreground text-sm text-center py-8">ไม่มีงาน onboarding — รอ HR มอบหมายเทมเพลต</p>
          )}
        </CardContent>
      </Card>

      {isHr && (
        <>
          <Card className={premiumCardClass}>
            <CardHeader className="border-b border-border/40 pb-4 flex flex-row items-center justify-between gap-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <ClipboardList className="size-5" />
                จัดการเทมเพลต
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4 pt-4">
              {templates.map((t) => (
                <div
                  key={t.id}
                  className="flex flex-wrap items-start justify-between gap-3 rounded-xl border border-border/50 bg-muted/20 p-4 hover:bg-muted/40 transition-colors"
                >
                  <div>
                    <p className="font-medium">{t.name}</p>
                    <ul className="text-sm text-muted-foreground list-disc pl-5 mt-1">
                      {t.tasks.map((task) => (
                        <li key={task.id}>{task.title}</li>
                      ))}
                    </ul>
                  </div>
                  <Button type="button" size="sm" variant="secondary" onClick={() => openEditEditor(t)}>
                    แก้ไข
                  </Button>
                </div>
              ))}
              {templates.length === 0 && (
                <p className="text-sm text-muted-foreground">ยังไม่มีเทมเพลต — กดสร้างเทมเพลตใหม่</p>
              )}
            </CardContent>
          </Card>
          <Card className={premiumCardClass}>
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary">มอบหมายเทมเพลต</CardTitle>
              <p className="text-sm text-muted-foreground">เลือกชุดงาน onboarding และพนักงานเป้าหมาย</p>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid gap-6 lg:grid-cols-[220px_minmax(0,1fr)]">
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="assign-template">เทมเพลต</Label>
                    <Select value={templateId} onValueChange={setTemplateId}>
                      <SelectTrigger id="assign-template" className="w-full">
                        <SelectValue placeholder="เลือกเทมเพลต" />
                      </SelectTrigger>
                      <SelectContent>
                        {templates.map((t) => (
                          <SelectItem key={t.id} value={t.id}>
                            {t.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  {selectedTemplate && (
                    <div className="rounded-lg border bg-muted/40 px-3 py-2.5 text-sm">
                      <p className="font-medium leading-snug">{selectedTemplate.name}</p>
                      <p className="text-xs text-muted-foreground mt-1">
                        {selectedTemplate.tasks.length} งานในเทมเพลต
                      </p>
                    </div>
                  )}
                </div>

                <div className="space-y-3 min-w-0">
                  <div className="flex items-center justify-between gap-2">
                    <Label htmlFor="assign-employee-search">พนักงาน</Label>
                    <Badge variant="secondary" className="shrink-0">
                      {filteredEmployees.length}/{employees.length}
                    </Badge>
                  </div>
                  <div className="relative">
                    <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                    <Input
                      id="assign-employee-search"
                      className="pl-9"
                      placeholder="ค้นหาชื่อ, username, อีเมล, แผนก..."
                      value={employeeSearch}
                      onChange={(e) => setEmployeeSearch(e.target.value)}
                    />
                  </div>
                  {selectedEmployee && (
                    <div className="flex items-start gap-2.5 rounded-lg border border-primary/25 bg-primary/5 px-3 py-2.5">
                      <UserCheck className="mt-0.5 h-4 w-4 shrink-0 text-primary" />
                      <div className="min-w-0 text-sm">
                        <p className="font-medium truncate">{selectedEmployee.fullName}</p>
                        <p className="text-xs text-muted-foreground">
                          {selectedEmployee.username} · {selectedEmployee.departmentName}
                        </p>
                      </div>
                    </div>
                  )}
                  <div className="rounded-lg border bg-card overflow-hidden">
                    <div className="max-h-[220px] overflow-y-auto divide-y">
                      {filteredEmployees.map((e) => (
                        <button
                          key={e.userId}
                          type="button"
                          onClick={() => setEmployeeId(e.userId)}
                          className={cn(
                            'w-full text-left px-3 py-2.5 text-sm transition-colors hover:bg-muted/80',
                            employeeId === e.userId && 'bg-muted'
                          )}
                        >
                          <p className="font-medium leading-snug">{e.fullName}</p>
                          <p className="text-xs text-muted-foreground mt-0.5">
                            {e.username} · {e.departmentName}
                          </p>
                        </button>
                      ))}
                      {filteredEmployees.length === 0 && (
                        <p className="px-3 py-8 text-sm text-muted-foreground text-center">ไม่พบพนักงาน</p>
                      )}
                    </div>
                  </div>
                </div>
              </div>

              <Separator />

              <div className="flex flex-col-reverse gap-3 sm:flex-row sm:items-center sm:justify-between">
                <p className="text-sm text-muted-foreground">
                  {canAssign
                    ? `พร้อมมอบหมาย "${selectedTemplate?.name}" ให้ ${selectedEmployee?.fullName}`
                    : 'เลือกเทมเพลตและพนักงานก่อนมอบหมาย'}
                </p>
                <Button className="sm:min-w-[140px] rounded-xl font-bold btn-premium" disabled={!canAssign} onClick={assign}>
                  มอบหมาย
                </Button>
              </div>
            </CardContent>
          </Card>
        </>
      )}

      <Dialog open={editorOpen} onOpenChange={setEditorOpen}>
        <DialogContent className="max-w-lg max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{editingTemplateId ? 'แก้ไขเทมเพลต' : 'สร้างเทมเพลตใหม่'}</DialogTitle>
          </DialogHeader>
          <div className="space-y-4 py-2">
            <div className="space-y-2">
              <Label>ชื่อเทมเพลต</Label>
              <Input value={templateName} onChange={(e) => setTemplateName(e.target.value)} />
            </div>
            <div className="space-y-2">
              <Label>รายการงาน</Label>
              <div className="space-y-2">
                {editTasks.map((row, index) => (
                  <div key={row.id ?? `new-${index}`} className="flex gap-2">
                    <Input
                      value={row.title}
                      onChange={(e) => updateTaskTitle(index, e.target.value)}
                      placeholder={`งานที่ ${index + 1}`}
                    />
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => removeTaskRow(index)}
                      disabled={editTasks.length <= 1}
                    >
                      ลบ
                    </Button>
                  </div>
                ))}
              </div>
              <Button type="button" variant="secondary" size="sm" onClick={addTaskRow}>
                เพิ่มงาน
              </Button>
            </div>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setEditorOpen(false)}>
              ยกเลิก
            </Button>
            <Button type="button" onClick={saveTemplate} disabled={saving}>
              {saving ? 'กำลังบันทึก...' : 'บันทึก'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </PageShell>
  )
}
