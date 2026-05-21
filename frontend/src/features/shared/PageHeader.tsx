import type { ReactNode } from 'react'
import { cn } from '@/lib/utils'

type PageHeaderProps = {
  icon: ReactNode
  title: string
  description?: string
  actions?: ReactNode
  className?: string
}

export function PageHeader({ icon, title, description, actions, className }: PageHeaderProps) {
  return (
    <div
      className={cn(
        'flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-border/50 pb-5',
        className
      )}
    >
      <div>
        <h2 className="text-3xl font-extrabold tracking-tight text-foreground flex items-center gap-3">
          <span className="p-2.5 rounded-xl bg-blue-500/10 text-primary shadow-inner shrink-0">
            {icon}
          </span>
          {title}
        </h2>
        {description && (
          <p className="text-sm text-muted-foreground mt-1.5 font-medium max-w-2xl">{description}</p>
        )}
      </div>
      {actions && <div className="shrink-0">{actions}</div>}
    </div>
  )
}

export function PageShell({ children, className }: { children: ReactNode; className?: string }) {
  return <div className={cn('max-w-6xl mx-auto space-y-6', className)}>{children}</div>
}

export function PageError({ message }: { message: string }) {
  if (!message) return null
  return (
    <div className="p-4 rounded-xl bg-destructive/10 border border-destructive/20 text-destructive text-sm font-semibold">
      {message}
    </div>
  )
}

export const premiumCardClass =
  'border border-blue-500/10 shadow-lg shadow-blue-500/[0.02] bg-card overflow-hidden'

export const filterBarClass =
  'rounded-xl border border-blue-500/10 bg-blue-500/[0.02] dark:bg-blue-500/5 p-4 flex flex-wrap gap-4 items-end'
