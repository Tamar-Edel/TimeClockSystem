import axios from 'axios'
import { getToken, removeToken } from '../utils/storage'

// API base URL is read from the environment variable set in .env.development.
// To change the backend URL, update VITE_API_BASE_URL in that file — do not hardcode it here.
const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
})

axiosClient.interceptors.request.use((config) => {
  const token = getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (!error.response) {
      // No response means the request never reached the server.
      // Common causes: backend not running, wrong port, or CORS preflight failure.
      console.error(
        `[API] Network error — could not reach the backend at "${import.meta.env.VITE_API_BASE_URL}". ` +
        `Check that the backend is running and that VITE_API_BASE_URL in .env.development is correct.`,
        error
      )
    }

    const isAuthEndpoint = error.config?.url?.startsWith('/auth/')
    if (error.response?.status === 401 && !isAuthEndpoint) {
      removeToken()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default axiosClient
