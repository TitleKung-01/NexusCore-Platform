import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { ExpenseClaim } from '@/types/api'

export function ExpenseListPage() {
  const [items, setItems] = useState<ExpenseClaim[]>([])
  const [error, setError] = useState('')

  useEffect(() => {
    api
      .get<ExpenseClaim[]>('/api/expense-claims?scope=mine')
      .then((r) => setItems(r.data))
      .catch((e) => setError(formatApiError(e, 'โหลดไม่สำเร็จ')))
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-semibold">เบิกค่าใช้จ่าย</h2>
        <Button asChild>
          <Link to="/expenses/new">ยื่นคำขอ</Link>
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
                <TableHead>หัวข้อ</TableHead>
                <TableHead>ยอดรวม</TableHead>
                <TableHead>สถานะ</TableHead>
                <TableHead></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {items.map((item) => (
                <TableRow key={item.id}>
                  <TableCell>{item.title}</TableCell>
                  <TableCell>{item.totalAmount.toLocaleString('th-TH')}</TableCell>
                  <TableCell>
                    <WorkflowStatusBadge status={item.status} />
                  </TableCell>
                  <TableCell>
                    <Button variant="link" size="sm" asChild>
                      <Link to={`/expenses/${item.id}`}>ดู</Link>
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
              {items.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} className="text-center text-muted-foreground">
                    ยังไม่มีคำขอ
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
