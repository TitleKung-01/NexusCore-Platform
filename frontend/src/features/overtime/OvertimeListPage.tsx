import { useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { api, formatApiError } from '@/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { WorkflowStatusBadge } from '@/features/shared/WorkflowStatusBadge'
import type { OvertimeRequest } from '@/types/api'
import { 
  Clock, 
  Plus, 
  Calendar, 
  Eye, 
  History, 
  BadgePercent,
  TrendingUp
} from 'lucide-react'

export function OvertimeListPage() {
  const navigate = useNavigate()
  const [items, setItems] = useState<OvertimeRequest[]>([])
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    setLoading(true)
    api
      .get<OvertimeRequest[]>('/api/overtime-requests?scope=mine')
      .then((r) => setItems(r.data))
      .catch((e) => setError(formatApiError(e, 'โหลดไม่สำเร็จ')))
      .finally(() => setLoading(false))
  }, [])

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      {/* Premium Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5">
        <div>
          <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
            <span className="p-2.5 rounded-xl bg-blue-500/10 text-primary shadow-inner">
              <Clock className="size-7" />
            </span>
            ล่วงเวลาของฉัน (My Overtime Requests)
          </h2>
          <p className="text-sm text-muted-foreground mt-1.5 font-medium">
            จัดการ ส่งคำขอ และติดตามสถานะการคำนวณเงินค่าทำงานล่วงเวลา (OT)
          </p>
        </div>
        <Button asChild className="rounded-xl font-bold shadow-md shadow-primary/20 hover:opacity-95 transition-all btn-premium shrink-0">
          <Link to="/overtime/new" className="flex items-center gap-1.5">
            <Plus className="size-4" />
            <span>ยื่นคำขอ OT ใหม่</span>
          </Link>
        </Button>
      </div>

      {error && (
        <div className="p-4 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm font-semibold">
          {error}
        </div>
      )}

      {/* OT Rate Cards at the Top */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Card className="border border-blue-500/10 bg-blue-500/5 hover:shadow-md transition-all">
          <CardContent className="p-5 flex items-center justify-between">
            <div className="space-y-1">
              <span className="text-xs font-bold text-muted-foreground uppercase tracking-wider block">อัตราวันทำงานปกติ</span>
              <h3 className="text-2xl font-black text-blue-600 dark:text-blue-400">1.5 เท่า <span className="text-xs text-muted-foreground font-semibold">ของอัตราปกติ</span></h3>
            </div>
            <div className="size-11 rounded-xl bg-white dark:bg-card flex items-center justify-center shadow-xs border border-border/40 shrink-0">
              <BadgePercent className="size-5 text-blue-500" />
            </div>
          </CardContent>
        </Card>
        
        <Card className="border border-indigo-500/10 bg-indigo-500/5 hover:shadow-md transition-all">
          <CardContent className="p-5 flex items-center justify-between">
            <div className="space-y-1">
              <span className="text-xs font-bold text-muted-foreground uppercase tracking-wider block">อัตราวันหยุดเสาร์-อาทิตย์</span>
              <h3 className="text-2xl font-black text-indigo-600 dark:text-indigo-400">3.0 เท่า <span className="text-xs text-muted-foreground font-semibold">ของอัตราปกติ</span></h3>
            </div>
            <div className="size-11 rounded-xl bg-white dark:bg-card flex items-center justify-center shadow-xs border border-border/40 shrink-0">
              <TrendingUp className="size-5 text-indigo-500" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main List Section */}
      <Card className="border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden">
        <CardHeader className="border-b border-border/40 pb-4 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <CardTitle className="text-lg font-bold text-primary flex items-center gap-2">
              <History className="size-5" />
              ประวัติคำขอปฏิบัติงานล่วงเวลา
            </CardTitle>
            <CardDescription className="text-xs font-semibold text-muted-foreground mt-0.5">
              แสดงคำขอทำงานล่วงเวลาที่ยื่นส่งอนุมัติทั้งหมดของคุณ
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <Table>
              <TableHeader className="bg-muted/40">
                <TableRow>
                  <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground pl-6 py-3">วันที่ปฏิบัติงาน</TableHead>
                  <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">ชั่วโมงสะสม</TableHead>
                  <TableHead className="font-extrabold text-[10px] uppercase tracking-wider text-foreground py-3">สถานะคำขอ</TableHead>
                  <TableHead className="py-3 text-right pr-6"></TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {items.map((item) => (
                  <TableRow key={item.id} className="hover:bg-muted/10 transition-colors">
                    <TableCell className="font-bold text-xs py-4 pl-6 text-foreground flex items-center gap-2.5">
                      <div className="size-8 rounded-lg bg-blue-500/5 border border-blue-500/5 flex items-center justify-center shrink-0">
                        <Calendar className="size-4 text-blue-500" />
                      </div>
                      <span>{item.workDate}</span>
                    </TableCell>
                    <TableCell className="font-semibold text-xs py-4 text-foreground">
                      <div className="flex items-center gap-1.5 text-muted-foreground">
                        <Clock className="size-3.5 text-indigo-500/70 shrink-0" />
                        <span className="text-foreground font-black">{item.hours}</span>
                        <span className="text-[10px] text-muted-foreground">ชั่วโมง</span>
                      </div>
                    </TableCell>
                    <TableCell className="py-4">
                      <WorkflowStatusBadge status={item.status} />
                    </TableCell>
                    <TableCell className="py-4 text-right pr-6">
                      <Button variant="ghost" size="sm" onClick={() => navigate(`/overtime/${item.id}`)} className="rounded-lg text-primary hover:bg-primary/10 hover:text-primary font-bold">
                        <Eye className="size-4 mr-1" />
                        <span>รายละเอียด</span>
                      </Button>
                    </TableCell>
                  </TableRow>
                ))}

                {items.length === 0 && !loading && (
                  <TableRow>
                    <TableCell colSpan={4} className="py-12 text-center">
                      <div className="flex flex-col items-center justify-center max-w-sm mx-auto space-y-3">
                        <div className="p-3.5 bg-blue-500/5 border border-blue-500/10 rounded-full text-blue-500">
                          <Clock className="size-8" />
                        </div>
                        <div>
                          <h4 className="text-sm font-extrabold text-foreground">ยังไม่มีการทำงานล่วงเวลา</h4>
                          <p className="text-xs text-muted-foreground mt-1">คุณยังไม่ได้ยื่นคำขออนุมัติชั่วโมง OT ในระบบ หากมีปฏิบัติภารกิจนอกเวลา สามารถยื่นคำขอใหม่ได้ทันที</p>
                        </div>
                        <Button asChild size="sm" className="rounded-lg mt-2">
                          <Link to="/overtime/new">ยื่นคำขอ OT แรก</Link>
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                )}

                {loading && (
                  <TableRow>
                    <TableCell colSpan={4} className="py-12 text-center">
                      <div className="flex flex-col items-center justify-center space-y-2">
                        <div className="animate-spin rounded-full h-7 w-7 border-b-2 border-primary"></div>
                        <p className="text-xs text-muted-foreground font-semibold">กำลังโหลดข้อมูลคำขอ OT...</p>
                      </div>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

