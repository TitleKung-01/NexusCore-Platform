import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { api, formatApiError, getStoredToken, setStoredToken } from '@/api'
import type { LoginResponse, MeResponse } from '@/types/api'

interface AuthContextValue {
  token: string | null
  me: MeResponse | null
  loading: boolean
  login: (username: string, password: string) => Promise<string | null>
  logout: () => void
  refreshMe: () => Promise<void>
  isApprover: boolean
  isHr: boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(getStoredToken())
  const [me, setMe] = useState<MeResponse | null>(null)
  const [loading, setLoading] = useState(!!getStoredToken())

  const refreshMe = useCallback(async () => {
    if (!getStoredToken()) {
      setMe(null)
      setLoading(false)
      return
    }
    try {
      const { data } = await api.get<MeResponse>('/api/me')
      setMe(data)
    } catch {
      setStoredToken(null)
      setToken(null)
      setMe(null)
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    if (token) void refreshMe()
    else setLoading(false)
  }, [token, refreshMe])

  const login = async (username: string, password: string) => {
    try {
      const { data } = await api.post<LoginResponse>('/api/auth/login', { username, password })
      setStoredToken(data.token)
      setToken(data.token)
      setLoading(true)
      return null
    } catch (err) {
      return formatApiError(err, 'เกิดข้อผิดพลาดในการเชื่อมต่อ')
    }
  }

  const logout = () => {
    setStoredToken(null)
    setToken(null)
    setMe(null)
  }

  const isApprover = useMemo(
    () => !!me && ['Manager', 'Hr', 'Admin'].includes(me.role),
    [me]
  )

  const isHr = useMemo(() => !!me && ['Hr', 'Admin'].includes(me.role), [me])

  const value = useMemo(
    () => ({ token, me, loading, login, logout, refreshMe, isApprover, isHr }),
    [token, me, loading, login, logout, refreshMe, isApprover, isHr]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
