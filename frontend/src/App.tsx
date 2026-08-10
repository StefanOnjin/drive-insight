import { useEffect, useState } from 'react'
import { getDriveById, getDrives } from './api/driveApi'
import './App.css'
import { DriveDetailPanel } from './components/DriveDetailPanel'
import { DriveList } from './components/DriveList'
import { EmptyState } from './components/EmptyState'
import { ErrorState } from './components/ErrorState'
import { LoadingState } from './components/LoadingState'
import type { DriveDetail, DriveListItem } from './types/drive'

function App() {
  const [drives, setDrives] = useState<DriveListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [selectedDriveId, setSelectedDriveId] = useState<string | null>(null)
  const [selectedDrive, setSelectedDrive] = useState<DriveDetail | null>(null)
  const [isDetailLoading, setIsDetailLoading] = useState(false)
  const [detailErrorMessage, setDetailErrorMessage] = useState<string | null>(null)

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

  function loadDriveDetail(driveId: string) {
    setSelectedDriveId(driveId)
    setSelectedDrive(null)
    setIsDetailLoading(true)
    setDetailErrorMessage(null)

    getDriveById(driveId)
      .then(setSelectedDrive)
      .catch((error: unknown) => {
        setDetailErrorMessage(error instanceof Error ? error.message : 'Unexpected error')
      })
      .finally(() => {
        setIsDetailLoading(false)
      })
  }

  function clearSelectedDrive() {
    setSelectedDriveId(null)
    setSelectedDrive(null)
    setDetailErrorMessage(null)
    setIsDetailLoading(false)
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
    <main className="app-shell">
      <header className="app-header">
        <div>
          <p className="eyebrow">Drive Insight</p>
          <h1>Drive history</h1>
        </div>
        <button type="button" className="secondary-button" onClick={() => loadDrives()}>
          Refresh
        </button>
      </header>

      <div className="dashboard-grid">
        <section className="drive-list-panel" aria-label="Drive list">
          {isLoading && <LoadingState />}
          {!isLoading && errorMessage && <ErrorState message={errorMessage} onRetry={() => loadDrives()} />}
          {!isLoading && !errorMessage && drives.length === 0 && <EmptyState />}
          {!isLoading && !errorMessage && drives.length > 0 && (
            <DriveList
              drives={drives}
              onSelectDrive={loadDriveDetail}
              selectedDriveId={selectedDriveId}
            />
          )}
        </section>

        <DriveDetailPanel
          detail={selectedDrive}
          errorMessage={detailErrorMessage}
          isLoading={isDetailLoading}
          onClose={clearSelectedDrive}
          onRetry={() => {
            if (selectedDriveId !== null) {
              loadDriveDetail(selectedDriveId)
            }
          }}
        />
      </div>
    </main>
  )
}

export default App
