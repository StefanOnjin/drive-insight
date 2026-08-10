import type { DriveDetail } from '../types/drive'
import {
  formatDateTime,
  formatDistance,
  formatDuration,
  formatEventType,
  formatEventValue,
  formatScore,
  formatSpeed,
  formatStatus,
  getScoreClassName,
} from '../utils/formatters'

type DriveDetailPanelProps = {
  detail: DriveDetail | null
  errorMessage: string | null
  isLoading: boolean
  onClose: () => void
  onRetry: () => void
}

export function DriveDetailPanel({
  detail,
  errorMessage,
  isLoading,
  onClose,
  onRetry,
}: DriveDetailPanelProps) {
  if (isLoading) {
    return (
      <aside className="detail-panel">
        <div className="detail-panel-header">
          <p className="eyebrow">Drive details</p>
        </div>
        <div className="state-panel" role="status">
          Loading drive details...
        </div>
      </aside>
    )
  }

  if (errorMessage !== null) {
    return (
      <aside className="detail-panel">
        <div className="detail-panel-header">
          <p className="eyebrow">Drive details</p>
          <button type="button" className="ghost-button" onClick={onClose}>
            Close
          </button>
        </div>
        <div className="state-panel state-panel-error" role="alert">
          <div>
            <strong>Unable to load drive details</strong>
            <p>{errorMessage}</p>
          </div>
          <button type="button" onClick={onRetry}>
            Retry
          </button>
        </div>
      </aside>
    )
  }

  if (detail === null) {
    return (
      <aside className="detail-panel detail-panel-empty">
        <p className="eyebrow">Drive details</p>
        <strong>Select a drive</strong>
        <p>Choose a drive from the list to inspect its analysis and detected events.</p>
      </aside>
    )
  }

  const analysis = detail.analysis

  return (
    <aside className="detail-panel">
      <div className="detail-panel-header">
        <div>
          <p className="eyebrow">Drive details</p>
          <h2>{formatDateTime(detail.startedAt)}</h2>
        </div>
        <button type="button" className="ghost-button" onClick={onClose}>
          Close
        </button>
      </div>

      <div className="detail-summary">
        <span className={`status-badge status-${detail.status}`}>{formatStatus(detail.status)}</span>
        <span>{detail.route.length} route points</span>
      </div>

      {analysis === null ? (
        <div className="state-panel">
          <strong>No analysis yet</strong>
          <p>This drive has not been analyzed.</p>
        </div>
      ) : (
        <>
          <section className="score-panel">
            <div>
              <p className="eyebrow">Driving score</p>
              <strong className={getScoreClassName(analysis.score)}>{formatScore(analysis.score)}</strong>
            </div>
            <p>{analysis.summary ?? 'No summary available.'}</p>
          </section>

          <dl className="detail-metrics">
            <div>
              <dt>Duration</dt>
              <dd>{formatDuration(analysis.durationSeconds)}</dd>
            </div>
            <div>
              <dt>Distance</dt>
              <dd>{formatDistance(analysis.distanceMeters)}</dd>
            </div>
            <div>
              <dt>Avg speed</dt>
              <dd>{formatSpeed(analysis.averageSpeedKmh)}</dd>
            </div>
            <div>
              <dt>Max speed</dt>
              <dd>{formatSpeed(analysis.maxSpeedKmh)}</dd>
            </div>
          </dl>

          <section>
            <h3>Event breakdown</h3>
            <dl className="event-breakdown">
              <div>
                <dt>Harsh acceleration</dt>
                <dd>{analysis.harshAccelerationCount}</dd>
              </div>
              <div>
                <dt>Harsh braking</dt>
                <dd>{analysis.harshBrakingCount}</dd>
              </div>
              <div>
                <dt>Sharp turns</dt>
                <dd>{analysis.sharpTurnCount}</dd>
              </div>
            </dl>
          </section>
        </>
      )}

      <section>
        <h3>Detected events</h3>
        {detail.events.length === 0 ? (
          <p className="muted-text">No detected events for this drive.</p>
        ) : (
          <div className="event-list">
            {detail.events.map((event) => (
              <article className="event-row" key={`${event.type}-${event.timestamp}`}>
                <div>
                  <strong>{formatEventType(event.type)}</strong>
                  <p>{formatDateTime(event.timestamp)}</p>
                </div>
                <div className="event-row-meta">
                  <span className={`severity-badge severity-${event.severity}`}>{event.severity}</span>
                  <span>{formatEventValue(event.value)}</span>
                </div>
              </article>
            ))}
          </div>
        )}
      </section>
    </aside>
  )
}
