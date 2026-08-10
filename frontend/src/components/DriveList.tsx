import type { DriveListItem as DriveListItemType } from '../types/drive'
import { DriveListItem } from './DriveListItem'

type DriveListProps = {
  drives: DriveListItemType[]
  onSelectDrive: (driveId: string) => void
  selectedDriveId: string | null
}

export function DriveList({ drives, onSelectDrive, selectedDriveId }: DriveListProps) {
  return (
    <div className="drive-list">
      {drives.map((drive) => (
        <DriveListItem
          key={drive.driveId}
          drive={drive}
          isSelected={drive.driveId === selectedDriveId}
          onSelect={onSelectDrive}
        />
      ))}
    </div>
  )
}
