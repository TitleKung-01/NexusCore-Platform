import * as React from 'react'
import * as DialogPrimitive from '@radix-ui/react-dialog'
import { X } from 'lucide-react'
import { cn } from '@/lib/utils'

function Dialog({ ...props }: React.ComponentProps<typeof DialogPrimitive.Root>) {
  return <DialogPrimitive.Root data-slot="dialog" {...props} />
}

function DialogTrigger({ ...props }: React.ComponentProps<typeof DialogPrimitive.Trigger>) {
  return <DialogPrimitive.Trigger data-slot="dialog-trigger" {...props} />
}

function DialogContent({
  className,
  children,
  ...props
}: React.ComponentProps<typeof DialogPrimitive.Content>) {
  return (
    <DialogPrimitive.Portal>
      <DialogPrimitive.Overlay className="fixed inset-0 z-50 bg-black/50 data-[state=open]:animate-in data-[state=closed]:animate-out" />
      <DialogPrimitive.Content
        className={cn(
          'bg-background fixed top-[50%] left-[50%] z-50 flex w-[calc(100%-2rem)] max-w-lg max-h-[min(90vh,calc(100%-2rem))] flex-col translate-x-[-50%] translate-y-[-50%] gap-0 overflow-hidden border p-0 shadow-lg duration-200 sm:rounded-xl',
          className
        )}
        {...props}
      >
        <div className="flex min-h-0 flex-1 flex-col overflow-y-auto p-6 pr-12">{children}</div>
        <DialogPrimitive.Close className="ring-offset-background focus:ring-ring absolute top-4 right-4 z-10 rounded-md p-1 opacity-70 transition-opacity hover:bg-muted hover:opacity-100 focus:ring-2 focus:ring-offset-2 focus:outline-hidden">
          <X className="size-4" />
          <span className="sr-only">Close</span>
        </DialogPrimitive.Close>
      </DialogPrimitive.Content>
    </DialogPrimitive.Portal>
  )
}

function DialogHeader({ className, ...props }: React.ComponentProps<'div'>) {
  return <div className={cn('flex flex-col gap-1.5 text-left', className)} {...props} />
}

function DialogTitle({ className, ...props }: React.ComponentProps<typeof DialogPrimitive.Title>) {
  return <DialogPrimitive.Title className={cn('text-lg leading-none font-semibold', className)} {...props} />
}

function DialogFooter({ className, ...props }: React.ComponentProps<'div'>) {
  return (
    <div
      className={cn('mt-4 flex flex-col-reverse gap-2 border-t border-border/40 pt-4 sm:flex-row sm:justify-end', className)}
      {...props}
    />
  )
}

export { Dialog, DialogTrigger, DialogContent, DialogHeader, DialogTitle, DialogFooter }
