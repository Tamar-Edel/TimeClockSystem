import { createContext, useContext, useEffect, useState } from 'react'
import * as authApi from '../api/authApi'
import axiosClient from '../api/axiosClient'
import { getToken, removeToken, saveToken } from '../utils/storage'

// Shape used everywhere: { fullName, email }


const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => getToken())
  const [user, setUser] = useState(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const existingToken = getToken()
    if (existingToken) {
      axiosClient
        .get('/auth/me')
        .then((response) => setUser(response.data))
        .catch(() => logout())
        .finally(() => setIsLoading(false))
    } else {
      setIsLoading(false)
    }
  }, [])

  async function loginUser(email, password) {
    const data = await authApi.login(email, password)
    saveToken(data.token)
    setToken(data.token)
    setUser({ fullName: data.fullName, email: data.email })
  }

  async function registerUser(fullName, email, password) {
    const data = await authApi.register(fullName, email, password)
    saveToken(data.token)
    setToken(data.token)
    setUser({ fullName: data.fullName, email: data.email })
  }

  function logout() {
    removeToken()
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ token, user, isLoading, loginUser, registerUser, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  return useContext(AuthContext)
}
