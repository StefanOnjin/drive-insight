import type { DriveListItem as DriveListItemType } from '../types/drive'
import {
  formatDateTime,
  formatDistance,
  formatDuration,
  formatScore,
  formatSpeed,
  formatStatus,
  getScoreClassName,
} from '../utils/formatters'

type DriveListItemProps = {
  drive: DriveListItemType
  isSelected: boolean
  onSelect: (driveId: string) => void
}

export function DriveListItem({ drive, isSelected, onSelect }: DriveListItemProps) {
  const scoreClassName = getScoreClassName(drive.score)

  return (
    <article className={`drive-row${isSelected ? ' drive-row-selected' : ''}`}>
      <div className="drive-row-main">
        <div>
          <h2>{formatDateTime(drive.startedAt)}</h2>
          <p>{drive.endedAt ? formatDuration(drive.durationSeconds) : 'Recording in progress'}</p>
        </div>
        <span className={`status-badge status-${drive.status}`}>{formatStatus(drive.status)}</span>
      </div>

      <dl className="drive-metrics">
        <div>
          <dt>Distance</dt>
          <dd>{formatDistance(drive.distanceMeters)}</dd>
        </div>
        <div>
          <dt>Avg speed</dt>
          <dd>{formatSpeed(drive.averageSpeedKmh)}</dd>
        </div>
        <div>
          <dt>Max speed</dt>
          <dd>{formatSpeed(drive.maxSpeedKmh)}</dd>
        </div>
        <div>
          <dt>Events</dt>
          <dd>{drive.eventCount}</dd>
        </div>
        <div>
          <dt>Score</dt>
          <dd className={scoreClassName}>{formatScore(drive.score)}</dd>
        </div>
      </dl>
      <button type="button" className="drive-select-button" onClick={() => onSelect(drive.driveId)}>
        View details
      </button>
    </article>
  )
}
