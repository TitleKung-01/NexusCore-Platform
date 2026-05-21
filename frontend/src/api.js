import axios from 'axios';

/**
 * Local:  VITE_API_URL=http://localhost:5000  (.env.development)
 * Docker: VITE_API_URL empty — calls /api on same host (Nginx → gateway-service)
 */
const baseURL = import.meta.env.VITE_API_URL ?? '';

export const api = axios.create({
  baseURL,
});

const TOKEN_KEY = 'jwt_token';

export function getStoredToken() {
  return localStorage.getItem(TOKEN_KEY);
}

export function setStoredToken(token) {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY);
  }
}

api.interceptors.request.use((config) => {
  const token = getStoredToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      setStoredToken(null);
    }
    return Promise.reject(error);
  }
);
