export default function ClockButtons({ currentShift, onClockIn, onClockOut, isLoading }) {
  if (!currentShift) {
    return (
      <button onClick={onClockIn} disabled={isLoading}>
        {isLoading ? 'Clocking in…' : 'Clock In'}
      </button>
    )
  }

  return (
    <button onClick={onClockOut} disabled={isLoading}>
      {isLoading ? 'Clocking out…' : 'Clock Out'}
    </button>
  )
}
