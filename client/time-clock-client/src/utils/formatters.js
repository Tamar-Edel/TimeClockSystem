export function formatDateTime(dateTimeOffset) {
  if (!dateTimeOffset) return '—'
  return new Date(dateTimeOffset).toLocaleString('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    timeZone: 'Europe/Zurich',
  })
}

export function formatDate(dateTimeOffset) {
  if (!dateTimeOffset) return '—'
  return new Date(dateTimeOffset).toLocaleDateString('en-GB', {
    timeZone: 'Europe/Zurich',
  })
}

export function formatDuration(minutes) {
  if (minutes == null) return '—'
  const h = Math.floor(minutes / 60)
  const m = Math.floor(minutes % 60)
  return `${h}h ${m.toString().padStart(2, '0')}m`
}
