import { useEffect, useState } from 'react'

// Ticks every second — used only for the visual clock display.
// Never used for Clock In / Clock Out timestamps (those come from the backend).
export function useLiveClock() {
  const [now, setNow] = useState(new Date())
  useEffect(() => {
    const t = setInterval(() => setNow(new Date()), 1000)
    return () => clearInterval(t)
  }, [])
  return now
}

// Returns the number of whole minutes elapsed since clockInAt.
// Updates every 10 seconds. Returns 0 when clockInAt is null.
export function useElapsed(clockInAt) {
  const [mins, setMins] = useState(0)
  useEffect(() => {
    if (!clockInAt) return
    const tick = () => {
      const diff = (Date.now() - new Date(clockInAt).getTime()) / 60000
      setMins(Math.floor(diff))
    }
    tick()
    const t = setInterval(tick, 10000)
    return () => clearInterval(t)
  }, [clockInAt])
  return mins
}
