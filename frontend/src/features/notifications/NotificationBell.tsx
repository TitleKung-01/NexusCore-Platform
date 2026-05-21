import { useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import { Link } from 'react-router-dom'
import { Bell } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import type { Notification, UnreadCountResponse } from '@/types/api'

const PANEL_WIDTH = 320

export function NotificationBell() {
  const [open, setOpen] = useState(false)
  const [unread, setUnread] = useState(0)
  const [items, setItems] = useState<Notification[]>([])
  const [loading, setLoading] = useState(false)
  const [panelPos, setPanelPos] = useState({ top: 0, left: 0 })
  const buttonRef = useRef<HTMLDivElement>(null)
  const panelRef = useRef<HTMLDivElement>(null)

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

  const updatePanelPosition = useCallback(() => {
    const btn = buttonRef.current
    if (!btn) return
    const rect = btn.getBoundingClientRect()
    let left = rect.right - PANEL_WIDTH
    const margin = 8
    if (left < margin) left = margin
    if (left + PANEL_WIDTH > window.innerWidth - margin) {
      left = window.innerWidth - PANEL_WIDTH - margin
    }
    setPanelPos({ top: rect.bottom + margin, left })
  }, [])

  useEffect(() => {
    loadCount()
    const id = setInterval(loadCount, 30000)
    return () => clearInterval(id)
  }, [loadCount])

  useEffect(() => {
    if (open) loadList()
  }, [open, loadList])

  useLayoutEffect(() => {
    if (!open) return
    updatePanelPosition()
    window.addEventListener('resize', updatePanelPosition)
    window.addEventListener('scroll', updatePanelPosition, true)
    return () => {
      window.removeEventListener('resize', updatePanelPosition)
      window.removeEventListener('scroll', updatePanelPosition, true)
    }
  }, [open, updatePanelPosition])

  useEffect(() => {
    const onClick = (e: MouseEvent) => {
      const target = e.target as Node
      if (
        buttonRef.current?.contains(target) ||
        panelRef.current?.contains(target)
      ) {
        return
      }
      setOpen(false)
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

  const panel =
    open &&
    createPortal(
      <div
        ref={panelRef}
        className="fixed z-[100] rounded-xl border border-blue-500/10 bg-card shadow-xl shadow-blue-500/10"
        style={{ top: panelPos.top, left: panelPos.left, width: PANEL_WIDTH }}
        role="dialog"
        aria-label="การแจ้งเตือน"
      >
        <div className="flex items-center justify-between border-b border-border/40 px-3 py-2.5">
          <span className="text-sm font-bold">การแจ้งเตือน</span>
          {unread > 0 && (
            <Button variant="link" size="sm" className="h-auto p-0 text-xs font-semibold" onClick={markAllRead}>
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
                className="border-b border-border/30 px-3 py-2.5 text-sm last:border-0 hover:bg-muted/50"
              >
                <div
                  className="cursor-pointer"
                  onClick={() => {
                    if (!n.isRead) void markRead(n.id)
                  }}
                >
                  <div className="flex items-start justify-between gap-2">
                    <p className="font-semibold leading-snug">{n.title}</p>
                    {!n.isRead && (
                      <Badge variant="secondary" className="text-[10px] shrink-0">
                        ใหม่
                      </Badge>
                    )}
                  </div>
                  <p className="text-muted-foreground text-xs line-clamp-2 mt-0.5">{n.body}</p>
                </div>
                {n.linkPath && (
                  <Link
                    to={n.linkPath}
                    className="inline-block mt-1.5 text-xs font-semibold text-primary hover:underline"
                    onClick={() => setOpen(false)}
                  >
                    เปิดรายละเอียด
                  </Link>
                )}
              </div>
            ))}
          {!loading && items.length === 0 && (
            <p className="p-6 text-sm text-muted-foreground text-center">ไม่มีการแจ้งเตือน</p>
          )}
        </div>
      </div>,
      document.body
    )

  return (
    <div className="relative shrink-0" ref={buttonRef}>
      <Button
        variant="ghost"
        size="icon"
        className="relative rounded-xl"
        onClick={() => {
          setOpen((o) => !o)
          if (!open) queueMicrotask(updatePanelPosition)
        }}
        aria-label="การแจ้งเตือน"
        aria-expanded={open}
      >
        <Bell className="size-5" />
        {unread > 0 && (
          <span className="absolute -top-0.5 -right-0.5 flex size-4 items-center justify-center rounded-full bg-destructive text-[10px] font-bold text-white">
            {unread > 9 ? '9+' : unread}
          </span>
        )}
      </Button>
      {panel}
    </div>
  )
}
