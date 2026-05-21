import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import type { LeaveRequest, LeaveType } from '@/types/api'

export function LeaveNewPage() {
  const navigate = useNavigate()
  const [types, setTypes] = useState<LeaveType[]>([])
  const [leaveTypeId, setLeaveTypeId] = useState('')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [reason, setReason] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [loadingTypes, setLoadingTypes] = useState(true)

  useEffect(() => {
    setLoadingTypes(true)
    api
      .get<LeaveType[]>('/api/leave-types')
      .then((r) => {
        setTypes(r.data)
        if (r.data[0]) setLeaveTypeId(r.data[0].id)
      })
      .catch((err) => toast.error(formatApiError(err, 'โหลดประเภทลาไม่สำเร็จ')))
      .finally(() => setLoadingTypes(false))
  }, [])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const { data } = await api.post<LeaveRequest>('/api/leave-requests', {
        leaveTypeId,
        startDate,
        endDate,
        reason,
      })
      await api.post(`/api/leave-requests/${data.id}/submit`)
      toast.success('ส่งคำขอลาแล้ว')
      navigate(`/leave/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="max-w-lg space-y-6">
      <h2 className="text-2xl font-semibold">ยื่นคำขอลา</h2>
      <Card>
        <CardHeader>
          <CardTitle>แบบฟอร์ม</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label>ประเภทลา</Label>
              <Select value={leaveTypeId} onValueChange={setLeaveTypeId} disabled={loadingTypes || types.length === 0}>
                <SelectTrigger>
                  <SelectValue placeholder={loadingTypes ? 'กำลังโหลด...' : types.length === 0 ? 'ไม่มีประเภทลา' : 'เลือกประเภท'} />
                </SelectTrigger>
                <SelectContent>
                  {types.map((t) => (
                    <SelectItem key={t.id} value={t.id}>
                      {t.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label>วันเริ่ม</Label>
                <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>วันสิ้นสุด</Label>
                <Input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
              </div>
            </div>
            <div className="space-y-2">
              <Label>เหตุผล</Label>
              <Textarea value={reason} onChange={(e) => setReason(e.target.value)} required />
            </div>
            <Button type="submit" disabled={submitting || loadingTypes || !leaveTypeId}>
              {submitting ? 'กำลังส่ง...' : 'ส่งคำขอ'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
