import { useEffect, useState } from 'react'
import { getDrives } from '../api/driveApi'
import { DriveList } from '../components/DriveList'
import { EmptyState } from '../components/EmptyState'
import { ErrorState } from '../components/ErrorState'
import { LoadingState } from '../components/LoadingState'
import type { DriveListItem } from '../types/drive'

type DriveListPageProps = {
  onSelectDrive: (driveId: string) => void
}

export function DriveListPage({ onSelectDrive }: DriveListPageProps) {
  const [drives, setDrives] = useState<DriveListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  function loadDrives() {
    setIsLoading(true)
    setErrorMessage(null)

    getDrives()
      .then(setDrives)
      .catch((error: unknown) => {
        setErrorMessage(error instanceof Error ? error.message : 'Unexpected error')
      })
      .finally(() => {
        setIsLoading(false)
      })
  }

  useEffect(() => {
    getDrives()
      .then(setDrives)
      .catch((error: unknown) => {
        setErrorMessage(error instanceof Error ? error.message : 'Unexpected error')
      })
      .finally(() => {
        setIsLoading(false)
      })
  }, [])

  return (
    <>
      <header className="app-header">
        <div>
          <p className="eyebrow">Drive Insight</p>
          <h1>Drive history</h1>
        </div>
        <button type="button" className="secondary-button" onClick={loadDrives}>
          Refresh
        </button>
      </header>

      <section aria-label="Drive list">
        {isLoading && <LoadingState />}
        {!isLoading && errorMessage && <ErrorState message={errorMessage} onRetry={loadDrives} />}
        {!isLoading && !errorMessage && drives.length === 0 && <EmptyState />}
        {!isLoading && !errorMessage && drives.length > 0 && (
          <DriveList drives={drives} onSelectDrive={onSelectDrive} />
        )}
      </section>
    </>
  )
}
