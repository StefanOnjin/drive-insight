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
  onSelect: (driveId: string) => void
}

export function DriveListItem({ drive, onSelect }: DriveListItemProps) {
  const scoreClassName = getScoreClassName(drive.score)

  return (
    <article className="drive-row">
      <div className="drive-row-main">
        <h3>{formatDateTime(drive.startedAt)}</h3>
        <p>{drive.endedAt ? formatDuration(drive.durationSeconds) : 'Recording in progress'}</p>
      </div>

      <div className="drive-table-cell drive-status-cell" data-label="Status">
        <span className={`status-badge status-${drive.status}`}>{formatStatus(drive.status)}</span>
      </div>
      <DriveValue label="Distance" value={formatDistance(drive.distanceMeters)} />
      <DriveValue label="Avg speed" value={formatSpeed(drive.averageSpeedKmh)} />
      <DriveValue label="Max speed" value={formatSpeed(drive.maxSpeedKmh)} />
      <DriveValue label="Events" value={drive.eventCount.toString()} />
      <div className="drive-table-cell score-cell" data-label="Score">
        <strong className={scoreClassName}>{formatScore(drive.score)}</strong>
      </div>
      <button
        type="button"
        className="drive-select-button"
        onClick={() => onSelect(drive.driveId)}
        aria-label={`Open drive from ${formatDateTime(drive.startedAt)}`}
      >
        View
      </button>
    </article>
  )
}

type DriveValueProps = {
  label: string
  value: string
}

function DriveValue({ label, value }: DriveValueProps) {
  return (
    <div className="drive-table-cell" data-label={label}>
      <span>{value}</span>
    </div>
  )
}
