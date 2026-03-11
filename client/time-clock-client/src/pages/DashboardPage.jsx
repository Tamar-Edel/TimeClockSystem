import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { clockIn, clockOut, getCurrentShift, getShiftHistory } from '../api/shiftsApi'
import ClockButtons from '../components/shifts/ClockButtons'
import CurrentShiftCard from '../components/shifts/CurrentShiftCard'
import ShiftTable from '../components/shifts/ShiftTable'
import { useAuth } from '../context/AuthContext'

export default function DashboardPage() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const [currentShift, setCurrentShift] = useState(null)
  const [history, setHistory] = useState([])
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState(null)

  useEffect(() => {
    loadData()
  }, [])

  async function loadData() {
    const [shiftResult, histResult] = await Promise.allSettled([getCurrentShift(), getShiftHistory()])

    if (shiftResult.status === 'fulfilled') {
      setCurrentShift(shiftResult.value?.shiftId ? shiftResult.value : null)
    } else {
      setCurrentShift(null)
    }

    if (histResult.status === 'fulfilled') {
      setHistory(histResult.value)
    } else {
      setError(histResult.reason?.response?.data?.message ?? 'Failed to load shift history.')
    }
  }

  async function handleClockIn() {
    setIsLoading(true)
    setError(null)
    try {
      await clockIn()
      await loadData()
    } catch (err) {
      setError(err.response?.data?.message ?? 'Clock in failed.')
    } finally {
      setIsLoading(false)
    }
  }

  async function handleClockOut() {
    setIsLoading(true)
    setError(null)
    try {
      await clockOut()
      await loadData()
    } catch (err) {
      setError(err.response?.data?.message ?? 'Clock out failed.')
    } finally {
      setIsLoading(false)
    }
  }

  function handleLogout() {
    logout()
    navigate('/login')
  }

  return (
    <div>
      <div>
        <h1>Welcome, {user?.fullName}</h1>
        <button onClick={handleLogout}>Logout</button>
      </div>
      {error && <p>{error}</p>}
      <CurrentShiftCard currentShift={currentShift} />
      <ClockButtons
        currentShift={currentShift}
        onClockIn={handleClockIn}
        onClockOut={handleClockOut}
        isLoading={isLoading}
      />
      <ShiftTable history={history} />
    </div>
  )
}
