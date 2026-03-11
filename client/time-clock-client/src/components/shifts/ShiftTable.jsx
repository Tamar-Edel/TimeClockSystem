import { formatDate, formatDateTime, formatDuration } from '../../utils/formatters'

export default function ShiftTable({ history }) {
  if (!history || history.length === 0) {
    return <p>No shifts recorded yet.</p>
  }

  return (
    <table>
      <thead>
        <tr>
          <th>Date</th>
          <th>Clock In</th>
          <th>Clock Out</th>
          <th>Duration</th>
          <th>Status</th>
        </tr>
      </thead>
      <tbody>
        {history.map((shift) => (
          <tr key={shift.shiftId}>
            <td>{formatDate(shift.clockInAt)}</td>
            <td>{formatDateTime(shift.clockInAt)}</td>
            <td>{shift.clockOutAt ? formatDateTime(shift.clockOutAt) : '—'}</td>
            <td>{formatDuration(shift.durationMinutes)}</td>
            <td>{shift.isOpen ? 'Open' : 'Closed'}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
