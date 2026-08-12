import type { DriveListItem as DriveListItemType } from '../types/drive'
import { DriveListItem } from './DriveListItem'

type DriveListProps = {
  drives: DriveListItemType[]
  onSelectDrive: (driveId: string) => void
}

export function DriveList({ drives, onSelectDrive }: DriveListProps) {
  return (
    <div className="drive-list">
      {drives.map((drive) => (
        <DriveListItem key={drive.driveId} drive={drive} onSelect={onSelectDrive} />
      ))}
    </div>
  )
}
