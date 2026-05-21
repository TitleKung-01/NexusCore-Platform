import { useCallback, useEffect, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bell } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import type { Notification, UnreadCountResponse } from '@/types/api'

export function NotificationBell() {
  const [open, setOpen] = useState(false)
  const [unread, setUnread] = useState(0)
  const [items, setItems] = useState<Notification[]>([])
  const [loading, setLoading] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  const loadCount = useCallback(() => {
    api
      .get<UnreadCountResponse>('/api/notifications/unread-count')
      .then((r) => setUnread(r.data.count))
      .catch(() => setUnread(0))
  }, [])

  const loadList = useCallback(() => {
    setLoading(true)
    api
      .get<Notification[]>('/api/notifications')
      .then((r) => setItems(r.data.slice(0, 20)))
      .catch(() => setItems([]))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    loadCount()
    const id = setInterval(loadCount, 30000)
    return () => clearInterval(id)
  }, [loadCount])

  useEffect(() => {
    if (open) loadList()
  }, [open, loadList])

  useEffect(() => {
    const onClick = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false)
    }
    if (open) document.addEventListener('mousedown', onClick)
    return () => document.removeEventListener('mousedown', onClick)
  }, [open])

  const markRead = async (id: string) => {
    try {
      await api.post(`/api/notifications/${id}/read`)
      loadCount()
      loadList()
    } catch (err) {
      toast.error(formatApiError(err, 'อ่านไม่สำเร็จ'))
    }
  }

  const markAllRead = async () => {
    try {
      await api.post('/api/notifications/read-all')
      loadCount()
      loadList()
    } catch (err) {
      toast.error(formatApiError(err, 'อ่านทั้งหมดไม่สำเร็จ'))
    }
  }

  return (
    <div className="relative" ref={ref}>
      <Button variant="ghost" size="icon" onClick={() => setOpen((o) => !o)} aria-label="การแจ้งเตือน">
        <Bell className="size-5" />
        {unread > 0 && (
          <span className="absolute -top-0.5 -right-0.5 flex size-4 items-center justify-center rounded-full bg-destructive text-[10px] text-white">
            {unread > 9 ? '9+' : unread}
          </span>
        )}
      </Button>
      {open && (
        <div className="absolute right-0 top-full z-50 mt-2 w-80 rounded-md border bg-card shadow-lg">
          <div className="flex items-center justify-between border-b px-3 py-2">
            <span className="text-sm font-medium">การแจ้งเตือน</span>
            {unread > 0 && (
              <Button variant="link" size="sm" className="h-auto p-0 text-xs" onClick={markAllRead}>
                อ่านทั้งหมด
              </Button>
            )}
          </div>
          <div className="max-h-72 overflow-y-auto">
            {loading && <p className="p-3 text-sm text-muted-foreground">กำลังโหลด...</p>}
            {!loading &&
              items.map((n) => (
                <div
                  key={n.id}
                  className="border-b px-3 py-2 text-sm last:border-0 hover:bg-muted/50 cursor-pointer"
                  onClick={() => {
                    if (!n.isRead) void markRead(n.id)
                    if (n.linkPath) setOpen(false)
                  }}
                >
                  <div className="flex items-start justify-between gap-2">
                    <p className="font-medium">{n.title}</p>
                    {!n.isRead && <Badge variant="secondary" className="text-[10px]">ใหม่</Badge>}
                  </div>
                  <p className="text-muted-foreground text-xs line-clamp-2">{n.body}</p>
                  {n.linkPath && (
                    <Link to={n.linkPath} className="text-xs text-primary underline" onClick={() => setOpen(false)}>
                      เปิด
                    </Link>
                  )}
                </div>
              ))}
            {!loading && items.length === 0 && (
              <p className="p-3 text-sm text-muted-foreground text-center">ไม่มีการแจ้งเตือน</p>
            )}
          </div>
        </div>
      )}
    </div>
  )
}
