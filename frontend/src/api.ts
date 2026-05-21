import axios from 'axios'

const baseURL = import.meta.env.VITE_API_URL ?? ''

export const api = axios.create({ baseURL })

const TOKEN_KEY = 'jwt_token'

export function getStoredToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setStoredToken(token: string | null) {
  if (token) localStorage.setItem(TOKEN_KEY, token)
  else localStorage.removeItem(TOKEN_KEY)
}

api.interceptors.request.use((config) => {
  const token = getStoredToken()
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) setStoredToken(null)
    return Promise.reject(error)
  }
)

export function formatApiError(err: unknown, fallback: string) {
  if (axios.isAxiosError(err)) {
    const msg = err.response?.data?.message ?? err.response?.data?.errors ?? fallback
    return typeof msg === 'object' ? JSON.stringify(msg) : String(msg)
  }
  return fallback
}
