import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function PrivateRoute() {
  const { token, isLoading } = useAuth()

  if (isLoading) return <p>Loading...</p>
  if (!token) return <Navigate to="/login" />
  return <Outlet />
}
