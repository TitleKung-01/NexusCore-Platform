import { useEffect, useState } from 'react'
import { CalendarDays, Palmtree, Plus, Trash2 } from 'lucide-react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import {
  filterBarClass,
  PageHeader,
  PageShell,
  premiumCardClass,
} from '@/features/shared/PageHeader'
import type { Holiday } from '@/types/api'

export function HolidaysPage() {
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [items, setItems] = useState<Holiday[]>([])
  const [date, setDate] = useState('')
  const [name, setName] = useState('')
  const [adding, setAdding] = useState(false)

  const load = () => {
    api
      .get<Holiday[]>('/api/holidays', { params: { year } })
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(load, [year])

  const add = async (e: React.FormEvent) => {
    e.preventDefault()
    setAdding(true)
    try {
      await api.post('/api/holidays', { date, name })
      toast.success('เพิ่มวันหยุดแล้ว')
      setDate('')
      setName('')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'เพิ่มไม่สำเร็จ'))
    } finally {
      setAdding(false)
    }
  }

  const remove = async (id: string) => {
    try {
      await api.delete(`/api/holidays/${id}`)
      toast.success('ลบแล้ว')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'ลบไม่สำเร็จ'))
    }
  }

  return (
    <PageShell>
      <PageHeader
        icon={<Palmtree className="size-7" />}
        title="วันหยุดประจำปี"
        description="กำหนดวันหยุดนักขัตฤกษ์และวันหยุดบริษัทสำหรับทั้งองค์กร"
      />

      <div className={filterBarClass}>
        <div className="space-y-2">
          <Label className="font-semibold text-xs uppercase tracking-wider text-muted-foreground">ปี</Label>
          <Input
            type="number"
            value={year}
            onChange={(e) => setYear(e.target.value)}
            className="w-32 rounded-xl"
            min={2020}
            max={2100}
          />
        </div>
        <div className="ml-auto flex items-center gap-2 rounded-xl bg-amber-500/10 border border-amber-500/15 px-4 py-2">
          <CalendarDays className="size-5 text-amber-600" />
          <span className="text-sm font-bold text-amber-700 dark:text-amber-400">
            {items.length} วันหยุดในปี {year}
          </span>
        </div>
      </div>

      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
            <Plus className="size-5" />
            เพิ่มวันหยุด
          </CardTitle>
          <CardDescription>ระบุวันที่และชื่อวันหยุดแล้วกดเพิ่ม</CardDescription>
        </CardHeader>
        <CardContent className="pt-5">
          <form onSubmit={add} className="flex flex-wrap gap-4 items-end">
            <div className="space-y-2">
              <Label className="font-semibold">วันที่</Label>
              <Input type="date" value={date} onChange={(e) => setDate(e.target.value)} required className="rounded-xl" />
            </div>
            <div className="space-y-2 flex-1 min-w-[200px]">
              <Label className="font-semibold">ชื่อวันหยุด</Label>
              <Input
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
                className="rounded-xl"
                placeholder="เช่น วันสงกรานต์"
              />
            </div>
            <Button type="submit" disabled={adding} className="rounded-xl btn-premium font-bold">
              <Plus className="size-4 mr-1" />
              {adding ? 'กำลังเพิ่ม...' : 'เพิ่มวันหยุด'}
            </Button>
          </form>
        </CardContent>
      </Card>

      <Card className={premiumCardClass}>
        <CardHeader className="border-b border-border/40 pb-4">
          <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
            <CalendarDays className="size-5" />
            รายการวันหยุด
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow className="bg-muted/30 hover:bg-muted/30">
                <TableHead className="font-bold">วันที่</TableHead>
                <TableHead className="font-bold">ชื่อ</TableHead>
                <TableHead className="w-[100px]" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((h) => (
                <TableRow key={h.id} className="hover:bg-blue-500/[0.02]">
                  <TableCell className="font-medium tabular-nums">{h.date}</TableCell>
                  <TableCell className="font-semibold">{h.name}</TableCell>
                  <TableCell>
                    <Button
                      size="sm"
                      variant="destructive"
                      className="rounded-lg"
                      onClick={() => remove(h.id)}
                    >
                      <Trash2 className="size-3.5 mr-1" />
                      ลบ
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={3} className="text-center text-muted-foreground py-10">
                    ไม่มีวันหยุดในปีนี้
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </PageShell>
  )
}
