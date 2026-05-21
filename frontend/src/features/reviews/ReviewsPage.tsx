import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { Textarea } from '@/components/ui/textarea'
import { useAuth } from '@/features/auth/AuthContext'
import type { PerformanceReview, ReviewCycle } from '@/types/api'

export function ReviewsPage() {
  const { isHr, isApprover } = useAuth()
  const [tab, setTab] = useState('mine')
  const [reviews, setReviews] = useState<PerformanceReview[]>([])
  const [cycles, setCycles] = useState<ReviewCycle[]>([])
  const [cycleName, setCycleName] = useState('')
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')

  const loadReviews = (scope: string) => {
    api
      .get<PerformanceReview[]>(`/api/reviews?scope=${scope}`)
      .then((r) => setReviews(r.data))
      .catch(() => setReviews([]))
  }

  useEffect(() => {
    api.get<ReviewCycle[]>('/api/reviews/cycles').then((r) => setCycles(r.data)).catch(() => setCycles([]))
  }, [])

  useEffect(() => {
    loadReviews(tab)
  }, [tab])

  const createCycle = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await api.post('/api/reviews/cycles', { name: cycleName, startDate, endDate })
      toast.success('สร้างรอบประเมินแล้ว')
      setCycleName('')
      const { data } = await api.get<ReviewCycle[]>('/api/reviews/cycles')
      setCycles(data)
    } catch (err) {
      toast.error(formatApiError(err, 'สร้างไม่สำเร็จ'))
    }
  }

  const submitSelf = async (id: string, comment: string, score: string) => {
    try {
      await api.post(`/api/reviews/${id}/self`, {
        selfComment: comment || null,
        selfScore: score ? Number(score) : null,
      })
      toast.success('บันทึกการประเมินตนเองแล้ว')
      loadReviews(tab)
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    }
  }

  const submitManager = async (id: string, comment: string, score: string) => {
    try {
      await api.post(`/api/reviews/${id}/manager`, {
        managerComment: comment || null,
        managerScore: score ? Number(score) : null,
      })
      toast.success('บันทึกการประเมินผู้จัดการแล้ว')
      loadReviews(tab)
    } catch (err) {
      toast.error(formatApiError(err, 'บันทึกไม่สำเร็จ'))
    }
  }

  return (
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">ประเมินผลงาน</h2>
      {isHr && (
        <Card>
          <CardHeader>
            <CardTitle>สร้างรอบประเมิน</CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={createCycle} className="flex flex-wrap gap-4 items-end">
              <div className="space-y-2">
                <Label>ชื่อรอบ</Label>
                <Input value={cycleName} onChange={(e) => setCycleName(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>เริ่ม</Label>
                <Input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} required />
              </div>
              <div className="space-y-2">
                <Label>สิ้นสุด</Label>
                <Input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} required />
              </div>
              <Button type="submit">สร้าง</Button>
            </form>
          </CardContent>
        </Card>
      )}
      {cycles.length > 0 && (
        <p className="text-sm text-muted-foreground">
          รอบที่เปิด: {cycles.filter((c) => c.isOpen).map((c) => c.name).join(', ') || '—'}
        </p>
      )}
      <Tabs value={tab} onValueChange={setTab}>
        <TabsList>
          <TabsTrigger value="mine">ของฉัน</TabsTrigger>
          {isApprover && <TabsTrigger value="team">ทีม</TabsTrigger>}
        </TabsList>
        <TabsContent value={tab}>
          <div className="space-y-4">
            {reviews.map((r) => (
              <ReviewCard
                key={r.id}
                review={r}
                showSelf={tab === 'mine'}
                showManager={tab === 'team' && isApprover}
                onSelf={submitSelf}
                onManager={submitManager}
              />
            ))}
            {reviews.length === 0 && <p className="text-muted-foreground">ไม่มีรายการประเมิน</p>}
          </div>
        </TabsContent>
      </Tabs>
    </div>
  )
}

function ReviewCard({
  review,
  showSelf,
  showManager,
  onSelf,
  onManager,
}: {
  review: PerformanceReview
  showSelf: boolean
  showManager: boolean
  onSelf: (id: string, c: string, s: string) => void
  onManager: (id: string, c: string, s: string) => void
}) {
  const [comment, setComment] = useState(review.selfComment ?? '')
  const [score, setScore] = useState(review.selfScore?.toString() ?? '')
  const [mgrComment, setMgrComment] = useState(review.managerComment ?? '')
  const [mgrScore, setMgrScore] = useState(review.managerScore?.toString() ?? '')

  return (
    <Card>
      <CardHeader>
        <CardTitle>
          {review.cycleName} — {review.employeeName}
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4 text-sm">
        {showSelf && (
          <div className="space-y-2">
            <p className="font-medium">ประเมินตนเอง</p>
            <Textarea value={comment} onChange={(e) => setComment(e.target.value)} placeholder="ความคิดเห็น" />
            <Input
              type="number"
              min={1}
              max={5}
              placeholder="คะแนน 1-5"
              value={score}
              onChange={(e) => setScore(e.target.value)}
            />
            <Button size="sm" onClick={() => onSelf(review.id, comment, score)}>
              บันทึก
            </Button>
          </div>
        )}
        {showManager && (
          <div className="space-y-2 border-t pt-4">
            <p className="font-medium">ประเมินโดยผู้จัดการ</p>
            {review.selfComment && <p className="text-muted-foreground">ตนเอง: {review.selfComment} ({review.selfScore ?? '—'})</p>}
            <Textarea value={mgrComment} onChange={(e) => setMgrComment(e.target.value)} />
            <Input
              type="number"
              min={1}
              max={5}
              value={mgrScore}
              onChange={(e) => setMgrScore(e.target.value)}
            />
            <Button size="sm" onClick={() => onManager(review.id, mgrComment, mgrScore)}>
              บันทึก
            </Button>
          </div>
        )}
        {!showSelf && !showManager && (
          <p>
            คะแนนตนเอง: {review.selfScore ?? '—'} · ผู้จัดการ: {review.managerScore ?? '—'}
          </p>
        )}
      </CardContent>
    </Card>
  )
}
