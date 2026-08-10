export type DriveStatus = 'recording' | 'completed' | 'failed'

export type EventType = 'harsh_acceleration' | 'harsh_braking' | 'sharp_turn'

export type EventSeverity = 'low' | 'medium' | 'high'

export type DriveListItem = {
  driveId: string
  startedAt: string
  endedAt: string | null
  status: DriveStatus
  durationSeconds: number | null
  distanceMeters: number | null
  averageSpeedKmh: number | null
  maxSpeedKmh: number | null
  score: number | null
  eventCount: number
}

export type RoutePoint = {
  timestamp: string
  latitude: number
  longitude: number
  speedKmh: number | null
}

export type DriveAnalysis = {
  durationSeconds: number
  distanceMeters: number
  averageSpeedKmh: number
  maxSpeedKmh: number
  harshAccelerationCount: number
  harshBrakingCount: number
  sharpTurnCount: number
  score: number
  summary: string | null
}

export type DetectedEvent = {
  type: EventType
  timestamp: string
  latitude: number
  longitude: number
  severity: EventSeverity
  value: number
}

export type DriveDetail = {
  driveId: string
  startedAt: string
  endedAt: string | null
  status: DriveStatus
  route: RoutePoint[]
  analysis: DriveAnalysis | null
  events: DetectedEvent[]
}
