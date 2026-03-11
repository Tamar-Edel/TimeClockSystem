export function formatDateTime(dateTimeOffset) {
  const date = new Date(dateTimeOffset)
  const time = date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false })
  const day = date.toLocaleDateString('en-GB')
  return `${time} · ${day}`
}

export function formatDate(dateTimeOffset) {
  return new Date(dateTimeOffset).toLocaleDateString('en-GB')
}

export function formatDuration(minutes) {
  if (minutes == null) return '—'
  const h = Math.floor(minutes / 60)
  const m = Math.floor(minutes % 60)
  if (h === 0) return `${m}m`
  return `${h}h ${m}m`
}
