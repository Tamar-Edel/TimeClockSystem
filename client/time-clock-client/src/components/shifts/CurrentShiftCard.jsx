import { formatDateTime } from '../../utils/formatters'

export default function CurrentShiftCard({ currentShift }) {
  if (!currentShift) {
    return <p>No open shift.</p>
  }

  return (
    <div>
      <p>Shift is open</p>
      <p>Clock in: {formatDateTime(currentShift.clockInAt)}</p>
      <p>Source: {currentShift.timeSource}</p>
    </div>
  )
}
