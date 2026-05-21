import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { useAuth } from '@/features/auth/AuthContext'
import type { EmployeeListItem, EmployeeOnboardingTask, OnboardingTemplate } from '@/types/api'

export function OnboardingPage() {
  const { isHr, me } = useAuth()
  const [tasks, setTasks] = useState<EmployeeOnboardingTask[]>([])
  const [templates, setTemplates] = useState<OnboardingTemplate[]>([])
  const [employees, setEmployees] = useState<EmployeeListItem[]>([])
  const [templateId, setTemplateId] = useState('')
  const [employeeId, setEmployeeId] = useState('')

  const loadTasks = () => {
    api
      .get<EmployeeOnboardingTask[]>('/api/onboarding/tasks')
      .then((r) => setTasks(r.data.sort((a, b) => a.sortOrder - b.sortOrder)))
      .catch(() => setTasks([]))
  }

  useEffect(() => {
    loadTasks()
    if (isHr) {
      api.get<OnboardingTemplate[]>('/api/onboarding/templates').then((r) => setTemplates(r.data)).catch(() => setTemplates([]))
      api.get<EmployeeListItem[]>('/api/employees').then((r) => setEmployees(r.data)).catch(() => setEmployees([]))
    }
  }, [isHr])

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

  const myTasks = tasks.filter((t) => t.employeeId === me?.userId)

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">Onboarding</h2>
      {isHr && (
        <Card>
          <CardHeader>
            <CardTitle>มอบหมายเทมเพลต (HR)</CardTitle>
          </CardHeader>
          <CardContent className="flex flex-wrap gap-4 items-end">
            <div className="space-y-2 min-w-[180px]">
              <Label>เทมเพลต</Label>
              <Select value={templateId} onValueChange={setTemplateId}>
                <SelectTrigger>
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
            <div className="space-y-2 min-w-[180px]">
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
            <Button onClick={assign}>มอบหมาย</Button>
          </CardContent>
        </Card>
      )}
      <Card>
        <CardHeader>
          <CardTitle>งานของฉัน</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {myTasks.map((t) => (
            <label key={t.id} className="flex items-center gap-3 rounded border p-3 cursor-pointer">
              <input
                type="checkbox"
                checked={t.isCompleted}
                onChange={(e) => toggleTask(t.id, e.target.checked)}
              />
              <span className={t.isCompleted ? 'line-through text-muted-foreground' : ''}>{t.title}</span>
            </label>
          ))}
          {myTasks.length === 0 && <p className="text-muted-foreground text-sm">ไม่มีงาน onboarding</p>}
        </CardContent>
      </Card>
      {isHr && templates.length > 0 && (
        <Card>
          <CardHeader>
            <CardTitle>เทมเพลต</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            {templates.map((t) => (
              <div key={t.id}>
                <p className="font-medium">{t.name}</p>
                <ul className="text-sm text-muted-foreground list-disc pl-5">
                  {t.tasks.map((task) => (
                    <li key={task.id}>{task.title}</li>
                  ))}
                </ul>
              </div>
            ))}
          </CardContent>
        </Card>
      )}
    </div>
  )
}
