import axios from 'axios';

/**
 * Local:  VITE_API_URL=http://localhost:5000  (.env.development)
 * Docker: VITE_API_URL empty — calls /api on same host (Nginx → gateway-service)
 */
const baseURL = import.meta.env.VITE_API_URL ?? '';

export const api = axios.create({
  baseURL,
});
