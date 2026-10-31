import { useEffect, useState } from 'react'
import { getDriveById } from '../api/driveApi'
import { DriveMap } from '../components/DriveMap'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import { SpeedChart } from '../components/SpeedChart'
import type { DriveDetail } from '../types/drive'
import {
  formatDateTime,
  formatDistance,
  formatDuration,
  formatEventType,
  formatEventValue,
  formatSpeed,
  formatStatus,
  getScoreClassName,
} from '../utils/formatters'

type DriveDetailsPageProps = {
  driveId: string
  onBack: () => void
}

export function DriveDetailsPage({ driveId, onBack }: DriveDetailsPageProps) {
  const [drive, setDrive] = useState<DriveDetail | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  function loadDrive() {
    setIsLoading(true)
    setErrorMessage(null)

    getDriveById(driveId)
      .then(setDrive)
      .catch((error: unknown) => {
        setErrorMessage(error instanceof Error ? error.message : 'Unexpected error')
      })
      .finally(() => {
        setIsLoading(false)
      })
  }

  useEffect(() => {
    getDriveById(driveId)
      .then(setDrive)
      .catch((error: unknown) => {
        setErrorMessage(error instanceof Error ? error.message : 'Unexpected error')
      })
      .finally(() => {
        setIsLoading(false)
      })
  }, [driveId])

  if (isLoading) {
    return (
      <>
        <PageHeader onBack={onBack} />
        <LoadingState />
      </>
    )
  }

  if (errorMessage !== null) {
    return (
      <>
        <PageHeader onBack={onBack} />
        <ErrorState message={errorMessage} onRetry={loadDrive} />
      </>
    )
  }

  if (drive === null) {
    return (
      <>
        <PageHeader onBack={onBack} />
        <div className="state-panel">
          <strong>Drive details unavailable</strong>
          <p>Select another drive from the history list.</p>
        </div>
      </>
    )
  }

  const analysis = drive.analysis

  return (
    <div className="details-page">
      <header className="details-header">
        <div className="details-navigation">
          <button type="button" className="ghost-button" onClick={onBack}>
            <span aria-hidden="true">&larr;</span>
            Back to drives
          </button>
          <span className={`status-badge status-${drive.status}`}>{formatStatus(drive.status)}</span>
        </div>

        <div className="details-title-row">
          <p className="eyebrow">Drive details</p>
          <h1>{formatDateTime(drive.startedAt)}</h1>
        </div>

        <div className="details-meta">
          <div>
            <span>Started</span>
            <strong>{formatDateTime(drive.startedAt)}</strong>
          </div>
          <div>
            <span>Ended</span>
            <strong>{drive.endedAt ? formatDateTime(drive.endedAt) : 'Recording in progress'}</strong>
          </div>
          <div>
            <span>Route data</span>
            <strong>{drive.route.length} GPS points</strong>
          </div>
        </div>
      </header>

      <DriveMap events={drive.events} route={drive.route} />

      {analysis === null ? (
        <div className="state-panel">
          <strong>No analysis yet</strong>
          <p>This drive has not been analyzed.</p>
        </div>
      ) : (
        <section className="analysis-overview" aria-label="Drive analysis overview">
          <div className="score-panel">
            <div>
              <p className="eyebrow">Driving score</p>
              <div className="score-value">
                <strong className={getScoreClassName(analysis.score)}>{analysis.score}</strong>
                <span>/100</span>
              </div>
            </div>
            <p>{analysis.summary ?? 'No summary available.'}</p>
          </div>

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

        </section>
      )}

      <div className={`details-grid${analysis === null ? ' details-grid-single' : ''}`}>
        {analysis !== null && (
          <section className="details-section">
            <div className="content-section-header">
              <div>
                <p className="eyebrow">Analysis</p>
                <h2>Event breakdown</h2>
              </div>
              <span>{drive.events.length} detected</span>
            </div>
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
        )}

        <section className="details-section">
          <div className="content-section-header">
            <div>
              <p className="eyebrow">Timeline</p>
              <h2>Detected events</h2>
            </div>
          </div>
          {drive.events.length === 0 ? (
            <p className="muted-text">No detected events for this drive.</p>
          ) : (
            <div className="event-list">
              {drive.events.map((event) => (
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
      </div>

      <SpeedChart route={drive.route} />
    </div>
  )
}

type PageHeaderProps = {
  onBack: () => void
}

function PageHeader({ onBack }: PageHeaderProps) {
  return (
    <header className="details-header">
      <button type="button" className="ghost-button" onClick={onBack}>
        <span aria-hidden="true">&larr;</span>
        Back to drives
      </button>
      <div>
        <p className="eyebrow">Drive details</p>
        <h1>Loading drive</h1>
      </div>
    </header>
  )
}
