export function formatDateTime(value: string) {
  return new Intl.DateTimeFormat('en', {
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    month: 'short',
    year: 'numeric',
  }).format(new Date(value))
}

export function formatDuration(seconds: number | null) {
  if (seconds === null) {
    return 'In progress'
  }

  if (seconds < 60) {
    return `${seconds}s`
  }

  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60

  if (remainingSeconds === 0) {
    return `${minutes}m`
  }

  return `${minutes}m ${remainingSeconds}s`
}

export function formatDistance(meters: number | null) {
  if (meters === null) {
    return '-'
  }

  if (meters < 1000) {
    return `${meters.toFixed(1)} m`
  }

  return `${(meters / 1000).toFixed(2)} km`
}

export function formatSpeed(speedKmh: number | null) {
  if (speedKmh === null) {
    return '-'
  }

  return `${speedKmh.toFixed(2)} km/h`
}

export function formatScore(score: number | null) {
  if (score === null) {
    return '-'
  }

  return `${score}/100`
}

export function formatStatus(status: string) {
  return status.replaceAll('_', ' ')
}

export function formatEventType(type: string) {
  return type.replaceAll('_', ' ')
}

export function formatEventValue(value: number) {
  return value.toFixed(2)
}

export function getScoreClassName(score: number | null) {
  if (score === null) {
    return 'score-muted'
  }

  if (score >= 85) {
    return 'score-good'
  }

  if (score >= 65) {
    return 'score-warning'
  }

  return 'score-risk'
}
