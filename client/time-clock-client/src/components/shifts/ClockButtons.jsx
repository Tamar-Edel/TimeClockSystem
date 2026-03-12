import { useRef } from 'react'

function PunchButton({ label, variant, onClick, disabled, loading }) {
  const btnRef = useRef(null)

  function handleClick(e) {
    if (disabled || loading) return

    // Ripple effect
    const btn = btnRef.current
    const rect = btn.getBoundingClientRect()
    const size = Math.max(rect.width, rect.height)
    const span = document.createElement('span')
    span.className = 'punch-ripple'
    span.style.cssText = `width:${size}px;height:${size}px;left:${e.clientX - rect.left - size / 2}px;top:${e.clientY - rect.top - size / 2}px;`
    btn.appendChild(span)
    setTimeout(() => span.remove(), 600)

    onClick()
  }

  return (
    <button
      ref={btnRef}
      className={`btn-clock btn-${variant}`}
      disabled={disabled || loading}
      onClick={handleClick}
    >
      {loading ? 'PROCESSING…' : label}
    </button>
  )
}

export default function ClockButtons({ currentShift, onClockIn, onClockOut, isLoading }) {
  if (!currentShift) {
    return (
      <PunchButton
        label="CLOCK IN"
        variant="in"
        onClick={onClockIn}
        disabled={false}
        loading={isLoading}
      />
    )
  }

  return (
    <PunchButton
      label="CLOCK OUT"
      variant="out"
      onClick={onClockOut}
      disabled={false}
      loading={isLoading}
    />
  )
}
