import type { DriveListItem } from '../types/drive'
import { formatDistance } from '../utils/formatters'

type DriveOverviewProps = {
  drives: DriveListItem[]
}

export function DriveOverview({ drives }: DriveOverviewProps) {
  const completedDrives = drives.filter((drive) => drive.status === 'completed').length
  const totalDistanceMeters = drives.reduce((total, drive) => total + (drive.distanceMeters ?? 0), 0)
  const scores = drives.flatMap((drive) => (drive.score === null ? [] : [drive.score]))
  const averageScore =
    scores.length === 0 ? null : scores.reduce((total, score) => total + score, 0) / scores.length

  return (
    <section className="drive-overview" aria-label="Drive overview">
      <OverviewItem label="Total drives" value={drives.length.toString()} />
      <OverviewItem label="Completed" value={completedDrives.toString()} />
      <OverviewItem label="Total distance" value={formatDistance(totalDistanceMeters)} />
      <OverviewItem label="Average score" value={averageScore === null ? '-' : `${averageScore.toFixed(1)}/100`} />
    </section>
  )
}

type OverviewItemProps = {
  label: string
  value: string
}

function OverviewItem({ label, value }: OverviewItemProps) {
  return (
    <div className="overview-item">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  )
}
