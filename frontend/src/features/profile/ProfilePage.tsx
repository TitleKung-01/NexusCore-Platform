import { useEffect, useState } from 'react'
import { Building2, KeyRound, Mail, Phone, Save, User } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Separator } from '@/components/ui/separator'
import { useAuth } from '@/features/auth/AuthContext'
import { PageHeader, PageShell, premiumCardClass } from '@/features/shared/PageHeader'
import type { MeResponse } from '@/types/api'

function getInitials(name?: string) {
  if (!name) return 'HR'
  return name
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase()
}

export function ProfilePage() {
  const { me, refreshMe } = useAuth()
  const [fullName, setFullName] = useState('')
  const [email, setEmail] = useState('')
  const [phone, setPhone] = useState('')
  const [saving, setSaving] = useState(false)
  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [changingPw, setChangingPw] = useState(false)

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

  const handleChangePassword = async (e: React.FormEvent) => {
    e.preventDefault()
    if (newPassword !== confirmPassword) {
      toast.error('รหัสผ่านใหม่ไม่ตรงกัน')
      return
    }
    setChangingPw(true)
    try {
      await api.post('/api/auth/change-password', { currentPassword, newPassword })
      toast.success('เปลี่ยนรหัสผ่านแล้ว')
      setCurrentPassword('')
      setNewPassword('')
      setConfirmPassword('')
    } catch (err) {
      toast.error(formatApiError(err, 'เปลี่ยนรหัสผ่านไม่สำเร็จ'))
    } finally {
      setChangingPw(false)
    }
  }

  if (!me) return null

  const infoItems = [
    { label: 'Username', value: me.username },
    { label: 'บทบาท', value: me.role },
    { label: 'แผนก', value: me.departmentName },
    { label: 'หัวหน้า', value: me.managerName ?? '—' },
  ]

  return (
    <PageShell className="max-w-4xl">
      <PageHeader
        icon={<User className="size-7" />}
        title="โปรไฟล์ของฉัน"
        description="จัดการข้อมูลส่วนตัว การติดต่อ และรหัสผ่านเข้าใช้งานระบบ"
      />

      <Card className={`${premiumCardClass} overflow-visible`}>
        <CardContent className="p-6">
          <div className="flex flex-col sm:flex-row items-center sm:items-start gap-5">
            <div className="size-20 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white text-2xl font-bold shadow-lg shadow-blue-500/25 border-2 border-white/20 shrink-0">
              {getInitials(me.fullName)}
            </div>
            <div className="text-center sm:text-left flex-1 min-w-0">
              <h3 className="text-xl font-bold truncate">{me.fullName}</h3>
              <p className="text-sm text-muted-foreground font-medium mt-0.5">{me.email}</p>
              <div className="flex flex-wrap justify-center sm:justify-start gap-2 mt-3">
                <span className="inline-flex items-center gap-1.5 rounded-lg bg-blue-500/10 text-primary px-2.5 py-1 text-xs font-semibold">
                  <Building2 className="size-3.5" />
                  {me.departmentName}
                </span>
                <span className="inline-flex items-center rounded-lg bg-muted px-2.5 py-1 text-xs font-semibold text-muted-foreground uppercase tracking-wide">
                  {me.role}
                </span>
              </div>
            </div>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-6 pt-6 border-t border-border/40">
            {infoItems.map((item) => (
              <div key={item.label} className="rounded-xl bg-muted/40 border border-border/30 px-3 py-2.5">
                <p className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">{item.label}</p>
                <p className="text-sm font-semibold mt-0.5 truncate">{item.value}</p>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <Mail className="size-5" />
              ข้อมูลส่วนตัว
            </CardTitle>
            <CardDescription>อัปเดตชื่อ อีเมล และเบอร์ติดต่อ</CardDescription>
          </CardHeader>
          <CardContent className="pt-5">
            <form onSubmit={handleSave} className="space-y-4">
              <div className="space-y-2">
                <Label className="font-semibold">ชื่อ-นามสกุล</Label>
                <Input
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  required
                  className="rounded-xl"
                />
              </div>
              <div className="space-y-2">
                <Label className="font-semibold">อีเมล</Label>
                <Input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="rounded-xl"
                />
              </div>
              <div className="space-y-2">
                <Label className="font-semibold flex items-center gap-1.5">
                  <Phone className="size-3.5 text-muted-foreground" />
                  เบอร์โทร
                </Label>
                <Input
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  className="rounded-xl"
                  placeholder="ไม่บังคับ"
                />
              </div>
              <Button
                type="submit"
                disabled={saving}
                className="w-full rounded-xl font-bold shadow-md shadow-primary/20 btn-premium"
              >
                <Save className="size-4 mr-1.5" />
                {saving ? 'กำลังบันทึก...' : 'บันทึกโปรไฟล์'}
              </Button>
            </form>
          </CardContent>
        </Card>

        <Card className={premiumCardClass}>
          <CardHeader className="border-b border-border/40 pb-4">
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <KeyRound className="size-5" />
              เปลี่ยนรหัสผ่าน
            </CardTitle>
            <CardDescription>ใช้รหัสผ่านที่แข็งแรงและไม่ซ้ำกับบัญชีอื่น</CardDescription>
          </CardHeader>
          <CardContent className="pt-5">
            <form onSubmit={handleChangePassword} className="space-y-4">
              <div className="space-y-2">
                <Label className="font-semibold">รหัสผ่านปัจจุบัน</Label>
                <Input
                  type="password"
                  value={currentPassword}
                  onChange={(e) => setCurrentPassword(e.target.value)}
                  required
                  autoComplete="current-password"
                  className="rounded-xl"
                />
              </div>
              <Separator className="bg-border/50" />
              <div className="space-y-2">
                <Label className="font-semibold">รหัสผ่านใหม่</Label>
                <Input
                  type="password"
                  value={newPassword}
                  onChange={(e) => setNewPassword(e.target.value)}
                  required
                  autoComplete="new-password"
                  className="rounded-xl"
                />
              </div>
              <div className="space-y-2">
                <Label className="font-semibold">ยืนยันรหัสผ่านใหม่</Label>
                <Input
                  type="password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  required
                  autoComplete="new-password"
                  className="rounded-xl"
                />
              </div>
              <Button type="submit" variant="outline" disabled={changingPw} className="w-full rounded-xl font-semibold">
                {changingPw ? 'กำลังเปลี่ยน...' : 'เปลี่ยนรหัสผ่าน'}
              </Button>
            </form>
          </CardContent>
        </Card>
      </div>
    </PageShell>
  )
}
