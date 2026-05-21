import { Badge } from '@/components/ui/badge'

export function LeaveStatusBadge({ status }: { status: string }) {
  const variant =
    status === 'Approved'
      ? 'default'
      : status === 'Rejected'
        ? 'destructive'
        : status === 'Pending'
          ? 'secondary'
          : 'outline'
  return <Badge variant={variant}>{status}</Badge>
}
