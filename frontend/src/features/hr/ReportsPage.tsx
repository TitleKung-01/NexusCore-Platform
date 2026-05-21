import { useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'

export function ReportsPage() {
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [downloading, setDownloading] = useState(false)

  const download = async () => {
    setDownloading(true)
    try {
      const { data } = await api.get<Blob>('/api/reports/leave-summary', {
        params: { year },
        responseType: 'blob',
      })
      const url = URL.createObjectURL(data)
      const a = document.createElement('a')
      a.href = url
      a.download = `leave-summary-${year}.csv`
      a.click()
      URL.revokeObjectURL(url)
      toast.success('ดาวน์โหลดแล้ว')
    } catch (err) {
      toast.error(formatApiError(err, 'ดาวน์โหลดไม่สำเร็จ'))
    } finally {
      setDownloading(false)
    }
  }

  return (
    <div className="max-w-lg space-y-6">
      <h2 className="text-2xl font-semibold">รายงาน</h2>
      <Card>
        <CardHeader>
          <CardTitle>สรุปวันลาประจำปี (CSV)</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="space-y-2">
            <Label>ปี</Label>
            <Input type="number" value={year} onChange={(e) => setYear(e.target.value)} min={2020} max={2100} />
          </div>
          <Button onClick={download} disabled={downloading}>
            {downloading ? 'กำลังดาวน์โหลด...' : 'ดาวน์โหลด CSV'}
          </Button>
        </CardContent>
      </Card>
    </div>
  )
}
