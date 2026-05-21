import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { LeaveStatusBadge } from '@/features/leave/statusBadge'
import type { LeaveRequest } from '@/types/api'

export function LeaveListPage() {
  const [items, setItems] = useState<LeaveRequest[]>([])
  const [error, setError] = useState('')

  useEffect(() => {
    api
      .get<LeaveRequest[]>('/api/leave-requests?scope=mine')
      .then((r) => setItems(r.data))
      .catch((e) => setError(formatApiError(e, 'โหลดไม่สำเร็จ')))
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold">คำขอลา</h2>
        <Button asChild>
          <Link to="/leave/new">ยื่นคำขอลา</Link>
        </Button>
      </div>
      {error && <p className="text-destructive text-sm">{error}</p>}
      <Card>
        <CardHeader>
          <CardTitle>รายการของฉัน</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ประเภท</TableHead>
                <TableHead>ช่วงวัน</TableHead>
                <TableHead>สถานะ</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell>{item.leaveTypeName}</TableCell>
                  <TableCell>
                    {item.startDate} — {item.endDate}
                  </TableCell>
                  <TableCell>
                    <LeaveStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell>
                    <Button variant="link" size="sm" asChild>
                      <Link to={`/leave/${item.id}`}>ดู</Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} className="text-muted-foreground text-center">
                    ยังไม่มีคำขอลา
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
