import type { DriveDetail, DriveListItem } from '../types/drive'

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'https://localhost:44394'

async function fetchJson<T>(path: string): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      Accept: 'application/json',
    },
  })

  if (!response.ok) {
    const message = await readErrorMessage(response)
    throw new Error(message)
  }

  return response.json() as Promise<T>
}

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { error?: string }
    return body.error ?? `Request failed with status ${response.status}`
  } catch {
    return `Request failed with status ${response.status}`
  }
}

export function getDrives() {
  return fetchJson<DriveListItem[]>('/api/v1/drives')
}

export function getDriveById(id: string) {
  return fetchJson<DriveDetail>(`/api/v1/drives/${id}`)
}
