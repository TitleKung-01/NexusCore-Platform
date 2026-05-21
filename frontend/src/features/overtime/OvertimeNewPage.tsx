import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Textarea } from '@/components/ui/textarea'
import type { OvertimeRequest } from '@/types/api'

export function OvertimeNewPage() {
  const navigate = useNavigate()
  const [workDate, setWorkDate] = useState('')
  const [hours, setHours] = useState('1')
  const [reason, setReason] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const { data } = await api.post<OvertimeRequest>('/api/overtime-requests', {
        workDate,
        hours: Number(hours),
        reason,
      })
      await api.post(`/api/overtime-requests/${data.id}/submit`)
      toast.success('ส่งคำขอแล้ว')
      navigate(`/overtime/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="max-w-lg space-y-6">
      <h2 className="text-2xl font-semibold">ยื่นคำขอล่วงเวลา</h2>
      <Card>
        <CardHeader>
          <CardTitle>แบบฟอร์ม</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label>วันที่ทำงาน</Label>
              <Input type="date" value={workDate} onChange={(e) => setWorkDate(e.target.value)} required />
            </div>
            <div className="space-y-2">
              <Label>จำนวนชั่วโมง</Label>
              <Input
                type="number"
                min={0.5}
                step={0.5}
                value={hours}
                onChange={(e) => setHours(e.target.value)}
                required
              />
            </div>
            <div className="space-y-2">
              <Label>เหตุผล</Label>
              <Textarea value={reason} onChange={(e) => setReason(e.target.value)} required />
            </div>
            <Button type="submit" disabled={submitting}>
              {submitting ? 'กำลังส่ง...' : 'ส่งคำขอ'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
