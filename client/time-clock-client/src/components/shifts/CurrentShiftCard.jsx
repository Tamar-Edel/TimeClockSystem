import { useElapsed } from '../../hooks/useLiveClock'
import { formatDateTime, formatDuration } from '../../utils/formatters'

export default function CurrentShiftCard({ currentShift }) {
  const isOpen = currentShift != null
  const elapsed = useElapsed(isOpen ? currentShift.clockInAt : null)

  return (
    <div className={`status-card${isOpen ? ' is-open' : ''}`}>
      <div className="status-header">
        <span className="status-label">Current Shift</span>
        {isOpen
          ? <span className="status-badge badge-open">● ACTIVE</span>
          : <span className="status-badge badge-closed">○ IDLE</span>
        }
      </div>

      {isOpen ? (
        <>
          <div className="status-row">
            <span className="status-row-label">CLOCKED IN AT</span>
            <span className="status-row-value">{formatDateTime(currentShift.clockInAt)}</span>
          </div>
          <div className="status-row">
            <span className="status-row-label">ELAPSED</span>
            <span className="elapsed">{formatDuration(elapsed)}</span>
          </div>
          <div className="status-row">
            <span className="status-row-label">TIME SOURCE</span>
            <span className="status-row-value" style={{ fontSize: '.7rem', color: 'var(--muted)' }}>
              {currentShift.timeSource}
            </span>
          </div>
        </>
      ) : (
        <p className="status-empty">No active shift</p>
      )}
    </div>
  )
}
