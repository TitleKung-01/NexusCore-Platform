import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useAuth } from '@/features/auth/AuthContext'
import type { MeResponse } from '@/types/api'

export function ProfilePage() {
  const { me, refreshMe } = useAuth()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [saving, setSaving] = useState(false)

  useEffect(() => {
    if (me) {
      setFullName(me.fullName)
      setEmail(me.email)
      setPhone(me.phone ?? '')
    }
  }, [me])

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault()
    setSaving(true)
    try {
      await api.put<MeResponse>('/api/me', { fullName, email, phone: phone || null })
      await refreshMe()
      toast.success('บันทึกโปรไฟล์แล้ว')
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    } finally {
      setSaving(false)
    }
  }

  if (!me) return null

  return (
    <div className="max-w-lg space-y-6">
      <h2 className="text-2xl font-semibold">โปรไฟล์</h2>
      <Card>
        <CardHeader>
          <CardTitle>ข้อมูลส่วนตัว</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSave} className="space-y-4">
            <div className="space-y-1 text-sm text-muted-foreground">
              <p>Username: {me.username}</p>
              <p>Role: {me.role}</p>
              <p>แผนก: {me.departmentName}</p>
              <p>หัวหน้า: {me.managerName ?? '—'}</p>
            </div>
            <div className="space-y-2">
              <Label>ชื่อ-นามสกุล</Label>
              <Input value={fullName} onChange={(e) => setFullName(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>อีเมล</Label>
              <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>เบอร์โทร</Label>
              <Input value={phone} onChange={(e) => setPhone(e.target.value)} />
            </div>
            <Button type="submit" disabled={saving}>
              {saving ? 'กำลังบันทึก...' : 'บันทึก'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
