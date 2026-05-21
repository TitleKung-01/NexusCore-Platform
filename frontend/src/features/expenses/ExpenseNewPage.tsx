import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { CreateExpenseLineItem, ExpenseClaim } from '@/types/api'

export function ExpenseNewPage() {
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [lines, setLines] = useState<CreateExpenseLineItem[]>([{ description: '', amount: 0 }])
  const [submitting, setSubmitting] = useState(false)

  const addLine = () => setLines([...lines, { description: '', amount: 0 }])
  const updateLine = (i: number, field: keyof CreateExpenseLineItem, value: string) => {
    const next = [...lines]
    if (field === 'amount') next[i] = { ...next[i], amount: Number(value) || 0 }
    else next[i] = { ...next[i], description: value }
    setLines(next)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const { data } = await api.post<ExpenseClaim>('/api/expense-claims', {
        title,
        lineItems: lines.filter((l) => l.description.trim()),
      })
      await api.post(`/api/expense-claims/${data.id}/submit`)
      toast.success('ส่งคำขอแล้ว')
      navigate(`/expenses/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="max-w-lg space-y-6">
      <h2 className="text-2xl font-semibold">ยื่นเบิกค่าใช้จ่าย</h2>
      <Card>
        <CardHeader>
          <CardTitle>แบบฟอร์ม</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label>หัวข้อ</Label>
              <Input value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
            <div className="space-y-3">
              <Label>รายการ</Label>
              {lines.map((line, i) => (
                <div key={i} className="grid grid-cols-2 gap-2">
                  <Input
                    placeholder="รายละเอียด"
                    value={line.description}
                    onChange={(e) => updateLine(i, 'description', e.target.value)}
                  />
                  <Input
                    type="number"
                    min={0}
                    step={0.01}
                    placeholder="จำนวนเงิน"
                    value={line.amount || ''}
                    onChange={(e) => updateLine(i, 'amount', e.target.value)}
                  />
                </div>
              ))}
              <Button type="button" variant="outline" size="sm" onClick={addLine}>
                + เพิ่มรายการ
              </Button>
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
