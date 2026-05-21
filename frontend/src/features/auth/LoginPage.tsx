import { useState } from 'react'
import { Navigate } from 'react-router-dom'
import { Building2, Lock, User, ArrowRight } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { useAuth } from '@/features/auth/AuthContext'

export function LoginPage() {
  const { token, login, loading } = useAuth()
  const [username, setUsername] = useState('employee')
  const [password, setPassword] = useState('password123')
  const [error, setError] = useState('')
  const [submitting, setSubmitting] = useState(false)

  if (token && !loading) return <Navigate to="/" replace />

  const handleSubmit = async (e?: React.FormEvent, customUser?: string, customPass?: string) => {
    if (e) e.preventDefault()
    setError('')
    setSubmitting(true)
    const err = await login(customUser || username, customPass || password)
    if (err) setError(err)
    setSubmitting(false)
  }

  const handleQuickLogin = (role: string) => {
    let u = 'employee'
    const p = 'password123'
    if (role === 'admin') u = 'admin'
    if (role === 'manager') u = 'mgr.eng'
    
    setUsername(u)
    setPassword(p)
    handleSubmit(undefined, u, p)
  }

  return (
    <div className="min-h-svh w-full flex items-center justify-center bg-[#f0f4f9] dark:bg-[#0c0f17] relative overflow-hidden p-4">
      {/* Decorative Gradient Glow Orbs */}
      <div className="absolute top-[-10%] left-[-10%] w-[50vw] h-[50vw] rounded-full bg-gradient-to-br from-blue-400/20 to-sky-300/0 blur-[120px] pointer-events-none" />
      <div className="absolute bottom-[-10%] right-[-10%] w-[50vw] h-[50vw] rounded-full bg-gradient-to-tr from-indigo-500/20 to-teal-400/0 blur-[120px] pointer-events-none" />
      
      <div className="w-full max-w-md relative z-10">
        <Card className="glass-panel border-white/40 shadow-2xl shadow-blue-500/5 dark:shadow-black/50 overflow-hidden">
          <div className="absolute top-0 inset-x-0 h-1.5 bg-gradient-to-r from-blue-500 via-indigo-500 to-sky-400" />
          
          <CardHeader className="text-center pt-8 pb-4">
            <div className="mx-auto size-12 rounded-2xl bg-gradient-to-br from-blue-500 to-indigo-600 flex items-center justify-center text-white shadow-lg shadow-blue-500/30 mb-4 animate-pulse">
              <Building2 className="size-6" />
            </div>
            <CardTitle className="text-3xl font-bold tracking-tight text-foreground bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent dark:from-blue-400 dark:to-indigo-400">
              HR-Lite
            </CardTitle>
            <CardDescription className="text-muted-foreground font-medium text-sm mt-1">
              ระบบจัดการและลงเวลาพนักงานภายในองค์กร
            </CardDescription>
          </CardHeader>
          
          <CardContent className="space-y-6">
            <form onSubmit={(e) => handleSubmit(e)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="username" className="text-xs font-semibold uppercase tracking-wider text-muted-foreground/80">
                  ชื่อผู้ใช้ (Username)
                </Label>
                <div className="relative">
                  <User className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
                  <Input 
                    id="username" 
                    value={username} 
                    onChange={(e) => setUsername(e.target.value)} 
                    className="pl-9 h-11 bg-white/50 dark:bg-black/20 border-muted focus:bg-white dark:focus:bg-black transition-all"
                    placeholder="ใส่ชื่อผู้ใช้งาน"
                  />
                </div>
              </div>
              
              <div className="space-y-2">
                <div className="flex justify-between items-center">
                  <Label htmlFor="password" className="text-xs font-semibold uppercase tracking-wider text-muted-foreground/80">
                    รหัสผ่าน (Password)
                  </Label>
                </div>
                <div className="relative">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 size-4 text-muted-foreground" />
                  <Input
                    id="password"
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    className="pl-9 h-11 bg-white/50 dark:bg-black/20 border-muted focus:bg-white dark:focus:bg-black transition-all"
                    placeholder="ใส่รหัสผ่าน"
                  />
                </div>
              </div>
              
              {error && (
                <div className="p-3 rounded-lg bg-destructive/10 border border-destructive/20 text-xs font-medium text-destructive animate-shake">
                  {error}
                </div>
              )}
              
              <Button type="submit" className="w-full h-11 font-semibold text-white shadow-lg bg-gradient-to-r from-blue-500 to-indigo-600 hover:from-blue-600 hover:to-indigo-700 transition-all border-0 rounded-xl" disabled={submitting}>
                {submitting ? (
                  <span className="flex items-center gap-2 justify-center">
                    กำลังเข้าสู่ระบบ...
                  </span>
                ) : (
                  <span className="flex items-center gap-2 justify-center">
                    เข้าสู่ระบบ <ArrowRight className="size-4" />
                  </span>
                )}
              </Button>
            </form>

            <div className="relative py-2">
              <div className="absolute inset-0 flex items-center">
                <span className="w-full border-t border-muted-foreground/10" />
              </div>
              <div className="relative flex justify-center text-xs uppercase">
                <span className="bg-[#ffffffcc] dark:bg-[#181c29cc] px-3 text-muted-foreground font-semibold tracking-wider">
                  เข้าใช้งานด่วน (Demo)
                </span>
              </div>
            </div>

            <div className="grid grid-cols-3 gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => handleQuickLogin('employee')}
                className="flex flex-col items-center py-2 h-auto rounded-xl hover:bg-blue-500/10 hover:text-blue-600 hover:border-blue-500/30 transition-all text-xs"
              >
                <span className="font-semibold text-foreground">พนักงาน</span>
                <span className="text-[10px] text-muted-foreground">Employee</span>
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => handleQuickLogin('manager')}
                className="flex flex-col items-center py-2 h-auto rounded-xl hover:bg-indigo-500/10 hover:text-indigo-600 hover:border-indigo-500/30 transition-all text-xs"
              >
                <span className="font-semibold text-foreground">ผู้จัดการ</span>
                <span className="text-[10px] text-muted-foreground">Manager</span>
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => handleQuickLogin('admin')}
                className="flex flex-col items-center py-2 h-auto rounded-xl hover:bg-teal-500/10 hover:text-teal-600 hover:border-teal-500/30 transition-all text-xs"
              >
                <span className="font-semibold text-foreground">ผู้ดูแลระบบ</span>
                <span className="text-[10px] text-muted-foreground">HR / Admin</span>
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}

