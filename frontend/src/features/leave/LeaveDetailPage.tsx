import { useEffect, useRef, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError, getStoredToken } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { LeaveAttachment, LeaveRequest } from '@/types/api'

export function LeaveDetailPage() {
  const { id } = useParams()
  const fileRef = useRef<HTMLInputElement>(null)
  const [item, setItem] = useState<LeaveRequest | null>(null)
  const [attachments, setAttachments] = useState<LeaveAttachment[]>([])
  const [loading, setLoading] = useState(true)
  const [uploading, setUploading] = useState(false)

  const load = () => {
    if (!id) return
    setLoading(true)
    Promise.all([
      api.get<LeaveRequest>(`/api/leave-requests/${id}`),
      api.get<LeaveAttachment[]>(`/api/leave-requests/${id}/attachments`),
    ])
      .then(([req, att]) => {
        setItem(req.data)
        setAttachments(att.data)
      })
      .catch(() => setItem(null))
      .finally(() => setLoading(false))
  }

  useEffect(load, [id])

  const cancel = async () => {
    try {
      await api.post(`/api/leave-requests/${id}/cancel`)
      toast.success('ยกเลิกแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ยกเลิกไม่สำเร็จ'))
    }
  }

  const uploadFile = async (file: File) => {
    setUploading(true)
    try {
      const form = new FormData()
      form.append('file', file)
      await api.post(`/api/leave-requests/${id}/attachments`, form)
      toast.success('อัปโหลดแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'อัปโหลดไม่สำเร็จ'))
    } finally {
      setUploading(false)
    }
  }

  const downloadAttachment = async (attachmentId: string, fileName: string) => {
    try {
      const token = getStoredToken()
      const base = import.meta.env.VITE_API_URL ?? ''
      const res = await fetch(`${base}/api/leave-requests/attachments/${attachmentId}/download`, {
        headers: token ? { Authorization: `Bearer ${token}` } : {},
      })
      if (!res.ok) throw new Error('failed')
      const blob = await res.blob()
      const url = URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = fileName
      a.click()
      URL.revokeObjectURL(url)
    } catch {
      toast.error('ดาวน์โหลดไม่สำเร็จ')
    }
  }

  if (loading) return <p className="text-muted-foreground">กำลังโหลด...</p>
  if (!item) return <p className="text-destructive">ไม่พบคำขอ</p>

  const canCancel = item.status === 'Draft' || item.status === 'Pending'
  const canUpload = item.status === 'Draft' || item.status === 'Pending'

  return (
    <div className="max-w-lg space-y-4">
      <Button variant="ghost" size="sm" asChild>
        <Link to="/leave">← กลับ</Link>
      </Button>
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>{item.leaveTypeName}</CardTitle>
          <LeaveStatusBadge status={item.status} />
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>
            <span className="text-muted-foreground">ช่วงวัน:</span> {item.startDate} — {item.endDate}
          </p>
          <p>
            <span className="text-muted-foreground">เหตุผล:</span> {item.reason}
          </p>
          {item.managerComment && (
            <p>
              <span className="text-muted-foreground">ความเห็นผู้อนุมัติ:</span> {item.managerComment}
            </p>
          )}
          {canCancel && (
            <Button variant="destructive" size="sm" onClick={cancel}>
              ยกเลิกคำขอ
            </Button>
          )}
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <CardTitle className="text-base">ไฟล์แนบ</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          <ul className="space-y-2 text-sm">
            {attachments.map((a) => (
              <li key={a.id} className="flex justify-between items-center">
                <span>{a.fileName}</span>
                <Button size="sm" variant="outline" onClick={() => downloadAttachment(a.id, a.fileName)}>
                  ดาวน์โหลด
                </Button>
              </li>
            ))}
            {attachments.length === 0 && <p className="text-muted-foreground">ยังไม่มีไฟล์แนบ</p>}
          </ul>
          {canUpload && (
            <>
              <input
                ref={fileRef}
                type="file"
                className="hidden"
                onChange={(e) => {
                  const f = e.target.files?.[0]
                  if (f) void uploadFile(f)
                  e.target.value = ''
                }}
              />
              <Button
                size="sm"
                variant="outline"
                disabled={uploading}
                onClick={() => fileRef.current?.click()}
              >
                {uploading ? 'กำลังอัปโหลด...' : 'แนบไฟล์'}
              </Button>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
