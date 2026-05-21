import { useEffect, useState } from 'react'
import { toast } from 'sonner'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import type { Holiday } from '@/types/api'

export function HolidaysPage() {
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [items, setItems] = useState<Holiday[]>([])
  const [date, setDate] = useState('')
  const [name, setName] = useState('')

  const load = () => {
    api
      .get<Holiday[]>('/api/holidays', { params: { year } })
      .then((r) => setItems(r.data))
      .catch(() => setItems([]))
  }

  useEffect(load, [year])

  const add = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await api.post('/api/holidays', { date, name })
      toast.success('เพิ่มวันหยุดแล้ว')
      setDate('')
      setName('')
      load()
    } catch (err) {
      toast.error(formatApiError(err, 'เพิ่มไม่สำเร็จ'))
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
    <div className="space-y-6">
      <h2 className="text-2xl font-semibold">วันหยุดประจำปี</h2>
      <div className="flex items-end gap-4">
        <div className="space-y-2">
          <Label>ปี</Label>
          <Input type="number" value={year} onChange={(e) => setYear(e.target.value)} className="w-32" />
        </div>
      </div>
      <Card>
        <CardHeader>
          <CardTitle>เพิ่มวันหยุด</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={add} className="flex flex-wrap gap-4 items-end">
            <div className="space-y-2">
              <Label>วันที่</Label>
              <Input type="date" value={date} onChange={(e) => setDate(e.target.value)} required />
            </div>
            <div className="space-y-2 flex-1 min-w-[200px]">
              <Label>ชื่อวันหยุด</Label>
              <Input value={name} onChange={(e) => setName(e.target.value)} required />
            </div>
            <Button type="submit">เพิ่ม</Button>
          </form>
        </CardContent>
      </Card>
      <Card>
        <CardHeader>
          <CardTitle>รายการวันหยุด</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>วันที่</TableHead>
                <TableHead>ชื่อ</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((h) => (
                <TableRow key={h.id}>
                  <TableCell>{h.date}</TableCell>
                  <TableCell>{h.name}</TableCell>
                  <TableCell>
                    <Button size="sm" variant="destructive" onClick={() => remove(h.id)}>
                      ลบ
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={3} className="text-center text-muted-foreground">
                    ไม่มีวันหยุด
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  )
}
