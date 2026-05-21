import { createContext, useContext, useState, type ReactNode } from 'react'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'

interface TabsContextValue {
  value: string
  setValue: (v: string) => void
}

const TabsContext = createContext<TabsContextValue | null>(null)

export function Tabs({
  defaultValue = '',
  value: controlledValue,
  onValueChange,
  children,
  className,
}: {
  defaultValue?: string
  value?: string
  onValueChange?: (v: string) => void
  children: ReactNode
  className?: string
}) {
  const [internal, setInternal] = useState(controlledValue ?? defaultValue)
  const value = controlledValue ?? internal
  const setValue = (v: string) => {
    setInternal(v)
    onValueChange?.(v)
  }
  return (
    <TabsContext.Provider value={{ value, setValue }}>
      <div className={className}>{children}</div>
    </TabsContext.Provider>
  )
}

export function TabsList({ children, className }: { children: ReactNode; className?: string }) {
  return <div className={cn('flex flex-wrap gap-2', className)}>{children}</div>
}

export function TabsTrigger({
  value,
  children,
  className,
}: {
  value: string
  children: ReactNode
  className?: string
}) {
  const ctx = useContext(TabsContext)
  if (!ctx) throw new Error('TabsTrigger must be inside Tabs')
  const active = ctx.value === value
  return (
    <Button
      type="button"
      size="sm"
      variant={active ? 'default' : 'outline'}
      className={className}
      onClick={() => ctx.setValue(value)}
    >
      {children}
    </Button>
  )
}

export function TabsContent({ value, children, className }: { value: string; children: ReactNode; className?: string }) {
  const ctx = useContext(TabsContext)
  if (!ctx) throw new Error('TabsContent must be inside Tabs')
  if (ctx.value !== value) return null
  return <div className={cn('mt-4', className)}>{children}</div>
}
