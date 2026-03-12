import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { clockIn, clockOut, getCurrentShift, getShiftHistory } from '../api/shiftsApi'
import ClockButtons from '../components/shifts/ClockButtons'
import CurrentShiftCard from '../components/shifts/CurrentShiftCard'
import ShiftTable from '../components/shifts/ShiftTable'
import { useAuth } from '../context/AuthContext'
import { useLiveClock } from '../hooks/useLiveClock'

function formatTime(date) {
  return date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false, timeZone: 'Europe/Zurich' })
}

function formatDateFull(date) {
  return date.toLocaleDateString('en-GB', { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric', timeZone: 'Europe/Zurich' })
}

export default function DashboardPage() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const now = useLiveClock()

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

  // Stats derived from history — closed shifts only
  const closedShifts = history.filter(s => !s.isOpen)
  const totalMins = closedShifts.reduce((acc, s) => acc + (s.durationMinutes ?? 0), 0)
  const avgMins = closedShifts.length ? Math.round(totalMins / closedShifts.length) : 0

  return (
    <div className="app">

      {/* ── Topbar ── */}
      <header className="topbar">
        <div className="topbar-brand">
          <div className="topbar-brand-dot" />
          TimeClock
        </div>
        <div className="topbar-right">
          <div className="topbar-user">
            Logged in as <span>{user?.fullName}</span>
          </div>
          <button className="btn-logout" onClick={handleLogout}>Sign Out</button>
        </div>
      </header>

      {/* ── Main Grid ── */}
      <main className="main">

        {/* ── Left Panel: clock + status + button ── */}
        <div className="panel-time">

          <div className="clock-display">
            <div className="clock-label">External Time Source</div>
            <div className="clock-time">{formatTime(now)}</div>
            <div className="clock-date">{formatDateFull(now)}</div>
            <div className="clock-source">
              <div className="source-dot" />
              timeapi.io
            </div>
          </div>

          <CurrentShiftCard currentShift={currentShift} />

          <ClockButtons
            currentShift={currentShift}
            onClockIn={handleClockIn}
            onClockOut={handleClockOut}
            isLoading={isLoading}
          />

          {error && (
            <div className="login-error" style={{ marginTop: '0' }}>{error}</div>
          )}

        </div>

        {/* ── Right Panel: stats + history ── */}
        <div className="panel-right">

          <div className="stats-row">
            <div className="stat-card accent">
              <div className="stat-label">Total Shifts</div>
              <div className="stat-value amber">{history.length}</div>
              <div className="stat-sub">all time</div>
            </div>
            <div className="stat-card">
              <div className="stat-label">Hours Logged</div>
              <div className="stat-value">
                {Math.floor(totalMins / 60)}
                <span style={{ fontSize: '1rem', color: 'var(--muted)' }}>h</span>
              </div>
              <div className="stat-sub">{totalMins % 60}m remaining</div>
            </div>
            <div className="stat-card">
              <div className="stat-label">Avg Shift</div>
              <div className="stat-value">
                {Math.floor(avgMins / 60)}
                <span style={{ fontSize: '1rem', color: 'var(--muted)' }}>h</span>
              </div>
              <div className="stat-sub">{avgMins % 60}m average</div>
            </div>
          </div>

          <ShiftTable history={history} />

        </div>

      </main>
    </div>
  )
}
