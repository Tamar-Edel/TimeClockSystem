import { useState } from 'react'
import { formatDateTime, formatDuration } from '../../utils/formatters'

export default function ShiftTable({ history }) {
  const [open, setOpen] = useState(false)

  return (
    <div className="history-panel">
      <div className="history-header" onClick={() => setOpen(o => !o)}>
        <span className="history-title">Shift History</span>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <span className="history-count">
            {history.length} record{history.length !== 1 ? 's' : ''}
          </span>
          <span className={`history-chevron${open ? ' open' : ''}`}>▼</span>
        </div>
      </div>

      {open && (
        <table>
          <thead>
            <tr>
              <th>#</th>
              <th>Clock In</th>
              <th>Clock Out</th>
              <th>Duration</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {history.length === 0 ? (
              <tr>
                <td colSpan={5} style={{ textAlign: 'center', color: 'var(--muted)', padding: '2rem' }}>
                  No shifts recorded
                </td>
              </tr>
            ) : (
              history.map((shift, i) => (
                <tr key={shift.shiftId}>
                  <td style={{ color: 'var(--muted)' }}>{String(i + 1).padStart(2, '0')}</td>
                  <td>{formatDateTime(shift.clockInAt)}</td>
                  <td>
                    {shift.clockOutAt
                      ? formatDateTime(shift.clockOutAt)
                      : <span style={{ color: 'var(--amber)' }}>—</span>
                    }
                  </td>
                  <td>{formatDuration(shift.durationMinutes)}</td>
                  <td>
                    {shift.isOpen
                      ? <span className="pill pill-open">Active</span>
                      : <span className="pill pill-closed">Closed</span>
                    }
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      )}
    </div>
  )
}
