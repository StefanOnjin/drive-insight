import { useState } from 'react'
import './App.css'
import { DriveDetailsPage } from './pages/DriveDetailsPage'
import { DriveListPage } from './pages/DriveListPage'

type AppView =
  | {
      name: 'list'
    }
  | {
      driveId: string
      name: 'details'
    }

function App() {
  const [view, setView] = useState<AppView>({ name: 'list' })

  return (
    <main className="app-shell">
      {view.name === 'list' && (
        <DriveListPage onSelectDrive={(driveId) => setView({ driveId, name: 'details' })} />
      )}
      {view.name === 'details' && (
        <DriveDetailsPage driveId={view.driveId} onBack={() => setView({ name: 'list' })} />
      )}
    </main>
  )
}

export default App
