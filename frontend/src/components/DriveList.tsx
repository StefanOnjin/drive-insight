import type { DriveListItem as DriveListItemType } from '../types/drive'
import { DriveListItem } from './DriveListItem'

type DriveListProps = {
  drives: DriveListItemType[]
  onSelectDrive: (driveId: string) => void
}

export function DriveList({ drives, onSelectDrive }: DriveListProps) {
  return (
    <div className="drive-table">
      <div className="drive-table-header" aria-hidden="true">
        <span>Drive</span>
        <span>Status</span>
        <span>Distance</span>
        <span>Avg speed</span>
        <span>Max speed</span>
        <span>Events</span>
        <span>Score</span>
        <span />
      </div>
      <div className="drive-list">
        {drives.map((drive) => (
          <DriveListItem key={drive.driveId} drive={drive} onSelect={onSelectDrive} />
        ))}
      </div>
    </div>
  )
}
