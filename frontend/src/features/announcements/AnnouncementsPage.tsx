import { useEffect, useState } from 'react'
import { Megaphone, Pencil, Plus } from 'lucide-react'
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
import { Textarea } from '@/components/ui/textarea'
import { Badge } from '@/components/ui/badge'
import { useAuth } from '@/features/auth/AuthContext'
import { PageHeader, PageShell, premiumCardClass } from '@/features/shared/PageHeader'
import { cn } from '@/lib/utils'
import type { Announcement } from '@/types/api'

export function AnnouncementsPage() {
  const { isHr } = useAuth()
  const [items, setItems] = useState<Announcement[]>([])
  const [open, setOpen] = useState(false)
  const [editId, setEditId] = useState<string | null>(null)
  const [title, setTitle] = useState('')
  const [body, setBody] = useState('')
  const [isActive, setIsActive] = useState(true)

  const load = () => {
    api
      .get<Announcement[]>('/api/announcements')
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(load, [])

  const resetForm = () => {
    setEditId(null)
    setTitle('')
    setBody('')
    setIsActive(true)
  }

  const openCreate = () => {
    resetForm()
    setOpen(true)
  }

  const openEdit = (a: Announcement) => {
    setEditId(a.id)
    setTitle(a.title)
    setBody(a.body)
    setIsActive(a.isActive)
    setOpen(true)
  }

  const handleOpenChange = (next: boolean) => {
    setOpen(next)
    if (!next) resetForm()
  }

  const save = async () => {
    try {
      if (editId) {
        await api.put(`/api/announcements/${editId}`, { title, body, isActive })
        toast.success('อัปเดตแล้ว')
      } else {
        await api.post('/api/announcements', { title, body })
        toast.success('เผยแพร่แล้ว')
      }
      setOpen(false)
      resetForm()
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    }
  }

  const activeCount = items.filter((a) => a.isActive).length

  return (
    <PageShell>
      <PageHeader
        icon={<Megaphone className="size-7" />}
        title="ประกาศองค์กร"
        description="ข่าวสารและประกาศสำคัญจาก HR ถึงพนักงานทุกคน"
        actions={
          isHr ? (
            <Button onClick={openCreate} className="rounded-xl font-bold btn-premium shadow-md shadow-primary/20">
              <Plus className="size-4 mr-1" />
              สร้างประกาศ
            </Button>
          ) : undefined
        }
      />

      <div className="flex gap-3">
        <div className="rounded-xl border border-blue-500/10 bg-blue-500/5 px-4 py-2 text-sm font-bold text-primary">
          ทั้งหมด {items.length} รายการ
        </div>
        <div className="rounded-xl border border-emerald-500/10 bg-emerald-500/5 px-4 py-2 text-sm font-bold text-emerald-700 dark:text-emerald-400">
          เปิดใช้งาน {activeCount}
        </div>
      </div>

      <div className="space-y-4">
        {items.map((a) => (
          <Card
            key={a.id}
            className={cn(
              premiumCardClass,
              !a.isActive && 'opacity-70',
              a.isActive && 'ring-1 ring-blue-500/10'
            )}
          >
            <CardHeader className="border-b border-border/40 pb-4 flex flex-row items-start justify-between gap-4 space-y-0">
              <div className="flex gap-3 min-w-0">
                <div className="size-10 rounded-xl bg-gradient-to-br from-blue-500/15 to-indigo-500/15 flex items-center justify-center shrink-0">
                  <Megaphone className="size-5 text-primary" />
                </div>
                <div className="min-w-0">
                  <CardTitle className="text-lg font-bold leading-snug">{a.title}</CardTitle>
                  <p className="text-xs text-muted-foreground mt-1 font-medium">
                    {new Date(a.publishedAtUtc).toLocaleString('th-TH', {
                      dateStyle: 'medium',
                      timeStyle: 'short',
                    })}
                  </p>
                </div>
              </div>
              <div className="flex gap-2 items-center shrink-0">
                {!a.isActive && (
                  <Badge variant="outline" className="font-semibold">
                    ปิดใช้งาน
                  </Badge>
                )}
                {isHr && (
                  <Button
                    type="button"
                    size="sm"
                    variant="outline"
                    className="rounded-lg"
                    onClick={(e) => {
                      e.stopPropagation()
                      openEdit(a)
                    }}
                  >
                    <Pencil className="size-3.5 mr-1" />
                    แก้ไข
                  </Button>
                )}
              </div>
            </CardHeader>
            <CardContent className="pt-4">
              <p className="text-sm whitespace-pre-wrap leading-relaxed text-foreground/90">{a.body}</p>
            </CardContent>
          </Card>
        ))}
        {items.length === 0 && (
          <Card className={premiumCardClass}>
            <CardContent className="py-16 text-center">
              <Megaphone className="size-12 mx-auto text-muted-foreground/40 mb-3" />
              <p className="text-muted-foreground font-medium">ยังไม่มีประกาศ</p>
              {isHr && (
                <Button variant="link" className="mt-2" onClick={openCreate}>
                  สร้างประกาศแรก
                </Button>
              )}
            </CardContent>
          </Card>
        )}
      </div>

      <Dialog open={open} onOpenChange={handleOpenChange}>
        <DialogContent className="rounded-2xl border border-blue-500/10 shadow-2xl p-0">
          <DialogHeader className="border-b border-border/40 pb-4 mb-0">
            <DialogTitle className="text-lg font-bold pr-2">
              {editId ? 'แก้ไขประกาศ' : 'สร้างประกาศ'}
            </DialogTitle>
          </DialogHeader>

          <div className="space-y-4 -mt-2">
            <div className="space-y-2">
              <Label className="font-semibold">หัวข้อ</Label>
              <Input
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
                className="rounded-xl"
                placeholder="หัวข้อประกาศ"
              />
            </div>
            <div className="space-y-2">
              <Label className="font-semibold">เนื้อหา</Label>
              <Textarea
                value={body}
                onChange={(e) => setBody(e.target.value)}
                rows={6}
                required
                className="rounded-xl min-h-[140px] resize-y"
                placeholder="รายละเอียดประกาศ"
              />
            </div>
            {editId && (
              <label className="flex items-center gap-3 text-sm font-medium rounded-xl border border-border/60 p-3 cursor-pointer hover:bg-muted/50 transition-colors">
                <input
                  type="checkbox"
                  checked={isActive}
                  onChange={(e) => setIsActive(e.target.checked)}
                  className="size-4 shrink-0 accent-primary"
                />
                <span>เปิดใช้งานประกาศนี้</span>
              </label>
            )}
          </div>

          <DialogFooter className="mt-0 pt-4">
            <Button type="button" variant="outline" className="rounded-xl" onClick={() => handleOpenChange(false)}>
              ยกเลิก
            </Button>
            <Button type="button" onClick={save} className="rounded-xl font-bold btn-premium">
              บันทึก
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </PageShell>
  )
}
