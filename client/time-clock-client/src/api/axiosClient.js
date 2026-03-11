import axios from 'axios'
import { getToken, removeToken } from '../utils/storage'

const axiosClient = axios.create({
  baseURL: 'http://localhost:5113/api',
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
    const isAuthEndpoint = error.config?.url?.startsWith('/auth/')
    if (error.response?.status === 401 && !isAuthEndpoint) {
      removeToken()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default axiosClient
