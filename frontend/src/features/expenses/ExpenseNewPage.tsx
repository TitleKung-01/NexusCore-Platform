import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { CreateExpenseLineItem, ExpenseClaim } from '@/types/api'
import { 
  Receipt, 
  Plus, 
  Trash2, 
  ClipboardList, 
  BookOpen, 
  CheckCircle2, 
  Coins
} from 'lucide-react'

export function ExpenseNewPage() {
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [lines, setLines] = useState<CreateExpenseLineItem[]>([{ description: '', amount: 0 }])
  const [submitting, setSubmitting] = useState(false)

  const addLine = () => setLines([...lines, { description: '', amount: 0 }])
  
  const removeLine = (index: number) => {
    if (lines.length === 1) {
      setLines([{ description: '', amount: 0 }])
    } else {
      setLines(lines.filter((_, i) => i !== index))
    }
  }

  const updateLine = (i: number, field: keyof CreateExpenseLineItem, value: string) => {
    const next = [...lines]
    if (field === 'amount') {
      next[i] = { ...next[i], amount: Number(value) || 0 }
    } else {
      next[i] = { ...next[i], description: value }
    }
    setLines(next)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setSubmitting(true)
    try {
      const validLines = lines.filter((l) => l.description.trim())
      if (validLines.length === 0) {
        toast.error('กรุณากรอกรายละเอียดรายการอย่างน้อย 1 รายการ')
        setSubmitting(false)
        return
      }

      const { data } = await api.post<ExpenseClaim>('/api/expense-claims', {
        title,
        lineItems: validLines,
      })
      await api.post(`/api/expense-claims/${data.id}/submit`)
      toast.success('ส่งคำขอเบิกค่าใช้จ่ายสำเร็จแล้ว')
      navigate(`/expenses/${data.id}`)
    } catch (err) {
      toast.error(formatApiError(err, 'ยื่นไม่สำเร็จ'))
    } finally {
      setSubmitting(false)
    }
  }

  const totalAmount = lines.reduce((sum, item) => sum + (Number(item.amount) || 0), 0)

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Premium Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5">
        <div>
          <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
            <span className="p-2 rounded-xl bg-blue-500/10 text-primary">
              <Receipt className="size-7" />
            </span>
            ยื่นเบิกค่าใช้จ่าย (Expense Claim)
          </h2>
          <p className="text-sm text-muted-foreground mt-1.5">
            สร้างใบเบิก กรอกรายการค่าใช้จ่ายแต่ละรายการ และแนบหลักฐานเพื่อส่งขออนุมัติเบิกจ่ายเงินคืน
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={() => navigate('/expenses')} className="rounded-xl">
          ประวัติการเบิกเงิน
        </Button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 items-start">
        {/* Left Column: Form Card */}
        <div className="lg:col-span-2 space-y-6">
          <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02]">
            <CardHeader className="border-b border-border/40 pb-4">
              <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
                <ClipboardList className="size-5" />
                รายละเอียดและรายการเบิกจ่าย
              </CardTitle>
              <CardDescription>กรอกหัวข้อใหญ่สำหรับการเบิกเงิน และจำแนกเป็นแต่ละรายการย่อย</CardDescription>
            </CardHeader>
            <CardContent className="pt-6">
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="space-y-2">
                  <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">หัวเรื่องการขอเบิกเงิน</Label>
                  <Input 
                    value={title} 
                    onChange={(e) => setTitle(e.target.value)} 
                    required 
                    placeholder="เช่น ค่าเดินทางพบคู่ค้าสัปดาห์นี้ หรือ ค่าวัสดุอุปกรณ์สำนักงานแผนก..."
                    className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-ring/30 focus:border-primary pl-3"
                  />
                </div>

                <div className="space-y-3.5">
                  <Label className="text-xs font-bold uppercase tracking-wider text-muted-foreground">รายการย่อย (Line Items)</Label>
                  
                  <div className="space-y-3">
                    {lines.map((line, i) => (
                      <div key={i} className="flex gap-2.5 items-center">
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-2.5 flex-1">
                          <div className="sm:col-span-2">
                            <Input
                              placeholder="รายละเอียด (เช่น ค่าทางด่วนบีทีเอส, ค่าปริ้นท์โปสเตอร์)"
                              value={line.description}
                              onChange={(e) => updateLine(i, 'description', e.target.value)}
                              required
                              className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-ring/30 focus:border-primary"
                            />
                          </div>
                          <div>
                            <div className="relative">
                              <Input
                                type="number"
                                min={0}
                                step={0.01}
                                placeholder="จำนวนเงิน"
                                value={line.amount || ''}
                                onChange={(e) => updateLine(i, 'amount', e.target.value)}
                                required
                                className="h-11 rounded-xl bg-muted/20 border-border/80 focus:ring-ring/30 focus:border-primary pr-8"
                              />
                              <span className="absolute right-3 top-1/2 -translate-y-1/2 text-xs font-bold text-muted-foreground">บาท</span>
                            </div>
                          </div>
                        </div>

                        {/* Custom Trash Button for UX Improvement */}
                        <Button
                          type="button"
                          variant="ghost"
                          onClick={() => removeLine(i)}
                          disabled={lines.length === 1 && !line.description && !line.amount}
                          className="rounded-xl h-11 px-3 text-muted-foreground hover:text-destructive hover:bg-destructive/10 shrink-0"
                        >
                          <Trash2 className="size-4" />
                        </Button>
                      </div>
                    ))}
                  </div>

                  <Button 
                    type="button" 
                    variant="outline" 
                    size="sm" 
                    onClick={addLine}
                    className="rounded-xl text-primary font-bold border-blue-500/20 hover:bg-primary/5 flex items-center gap-1.5 mt-2 h-9 px-4 text-xs"
                  >
                    <Plus className="size-3.5" />
                    เพิ่มรายการเบิกใหม่
                  </Button>
                </div>

                <div className="pt-2 border-t border-border/20 flex justify-end">
                  <Button 
                    type="submit" 
                    disabled={submitting}
                    className="w-full md:w-auto px-8 h-11 text-sm font-semibold rounded-xl bg-primary text-primary-foreground shadow-md shadow-primary/20 hover:opacity-90 transition-all btn-premium shrink-0"
                  >
                    {submitting ? 'กำลังส่งแบบเบิก...' : 'ส่งคำขอเบิกค่าใช้จ่าย'}
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </div>

        {/* Right Column: Total summary & guidelines */}
        <div className="space-y-6">
          {/* Total Amount Claim Summary Display */}
          <Card className="bg-gradient-to-br from-blue-600 via-indigo-600 to-indigo-700 text-white shadow-xl shadow-blue-500/20 border-0 rounded-2xl relative overflow-hidden">
            {/* Ambient Background Glow Orbs */}
            <div className="absolute -right-10 -top-10 size-32 bg-white/10 rounded-full blur-xl" />
            <div className="absolute -left-10 -bottom-10 size-32 bg-sky-400/20 rounded-full blur-xl" />
            
            <CardContent className="p-6 space-y-5 relative">
              <div className="flex items-center gap-2 text-white/80">
                <Coins className="size-4" />
                <span className="text-[10px] font-extrabold uppercase tracking-widest">สรุปยอดรวมขอเบิก</span>
              </div>
              
              <div>
                <span className="text-[10px] text-white/70 font-semibold block">ยอดเบิกทั้งสิ้น (Total Estimate)</span>
                <h3 className="text-3xl font-black mt-2 font-mono flex items-baseline gap-1 tracking-tight select-all">
                  {totalAmount.toLocaleString('th-TH', { minimumFractionDigits: 2 })}
                  <span className="text-sm font-bold text-white/95 ml-1">บาท</span>
                </h3>
              </div>

              <div className="pt-3 border-t border-white/10 text-[10px] text-white/70 font-medium">
                ประกอบด้วยจำนวนรายการเบิกย่อย: <span className="font-bold text-white">{lines.filter(l => l.description.trim()).length} รายการ</span>
              </div>
            </CardContent>
          </Card>

          {/* Corporate Reimbursement Guidelines */}
          <Card className="border border-blue-500/10 shadow-md bg-muted/5">
            <CardHeader className="pb-3 border-b border-border/40">
              <CardTitle className="text-sm font-extrabold uppercase tracking-wider text-muted-foreground flex items-center gap-2">
                <BookOpen className="size-4 text-blue-500" />
                ระเบียบและนโยบายเบิกจ่าย
              </CardTitle>
            </CardHeader>
            <CardContent className="pt-4 text-xs font-medium space-y-3.5 text-muted-foreground">
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การเบิกจ่ายทุกประเภทจะต้องมีหลักฐานใบเสร็จรับเงิน หรือใบกำกับภาษีเต็มรูปแบบกำกับเสมอ</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>การเบิกค่าเลี้ยงรับรองหรืออาหารกลางวันระหว่างปฏิบัติภารกิจภายนอก ต้องไม่เกิน 400 บาทต่อมื้อ</p>
              </div>
              <div className="flex items-start gap-2.5">
                <CheckCircle2 className="size-4 text-emerald-500 shrink-0 mt-0.5" />
                <p>กรุณาส่งแบบฟอร์มการเบิกจ่ายคืนพร้อมหลักฐานใบเสร็จภายในวันที่ 25 ของเดือนปัจจุบัน</p>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
