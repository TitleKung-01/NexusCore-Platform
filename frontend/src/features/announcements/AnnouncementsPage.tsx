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
import { Textarea } from '@/components/ui/textarea'
import { Badge } from '@/components/ui/badge'
import { useAuth } from '@/features/auth/AuthContext'
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

  const openCreate = () => {
    setEditId(null)
    setTitle('')
    setBody('')
    setIsActive(true)
    setOpen(true)
  }

  const openEdit = (a: Announcement) => {
    setEditId(a.id)
    setTitle(a.title)
    setBody(a.body)
    setIsActive(a.isActive)
    setOpen(true)
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
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    }
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold">ประกาศ</h2>
        {isHr && (
          <Button onClick={openCreate}>สร้างประกาศ</Button>
        )}
      </div>
      <div className="space-y-4">
        {items.map((a) => (
          <Card key={a.id} className={!a.isActive ? 'opacity-60' : ''}>
            <CardHeader className="flex flex-row items-start justify-between gap-4">
              <div>
                <CardTitle>{a.title}</CardTitle>
                <p className="text-xs text-muted-foreground mt-1">
                  {new Date(a.publishedAtUtc).toLocaleString('th-TH')}
                </p>
              </div>
              <div className="flex gap-2 items-center">
                {!a.isActive && <Badge variant="outline">ปิดใช้งาน</Badge>}
                {isHr && (
                  <Button size="sm" variant="outline" onClick={() => openEdit(a)}>
                    แก้ไข
                  </Button>
                )}
              </div>
            </CardHeader>
            <CardContent>
              <p className="text-sm whitespace-pre-wrap">{a.body}</p>
            </CardContent>
          </Card>
        ))}
        {items.length === 0 && <p className="text-muted-foreground">ยังไม่มีประกาศ</p>}
      </div>
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>{editId ? 'แก้ไขประกาศ' : 'สร้างประกาศ'}</DialogTitle>
          </DialogHeader>
          <div className="space-y-3">
            <div className="space-y-2">
              <Label>หัวข้อ</Label>
              <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>เนื้อหา</Label>
              <Textarea value={body} onChange={(e) => setBody(e.target.value)} rows={5} required />
            </div>
            {editId && (
              <label className="flex items-center gap-2 text-sm">
                <input type="checkbox" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} />
                เปิดใช้งาน
              </label>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>
              ยกเลิก
            </Button>
            <Button onClick={save}>บันทึก</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  )
}
