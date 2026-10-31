import { useEffect } from 'react'
import { CircleMarker, MapContainer, Marker, Polyline, Popup, TileLayer, useMap } from 'react-leaflet'
import type { DetectedEvent, RoutePoint } from '../types/drive'
import { formatDateTime, formatEventType, formatEventValue, formatSpeed } from '../utils/formatters'
import 'leaflet/dist/leaflet.css'

type DriveMapProps = {
  events: DetectedEvent[]
  route: RoutePoint[]
}

type LatLngTuple = [number, number]

export function DriveMap({ events, route }: DriveMapProps) {
  const routePositions = route.map<LatLngTuple>((point) => [point.latitude, point.longitude])

  if (routePositions.length === 0) {
    return (
      <section className="map-empty-state">
        <div>
          <p className="eyebrow">Map</p>
          <strong>No route data</strong>
          <p>This drive does not have usable GPS points.</p>
        </div>
      </section>
    )
  }

  const center = routePositions[Math.floor(routePositions.length / 2)]

  return (
    <section className="route-section" aria-label="Drive route map">
      <div className="content-section-header">
        <div>
          <p className="eyebrow">Route</p>
          <h2>Drive map</h2>
        </div>
        <span>{route.length} GPS points</span>
      </div>
      <div className="map-section">
        <MapContainer center={center} className="drive-map" scrollWheelZoom zoom={15}>
          <TileLayer
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          <Polyline pathOptions={{ color: '#2563eb', weight: 5 }} positions={routePositions} />
          {route.map((point) => (
            <CircleMarker
              center={[point.latitude, point.longitude]}
              key={point.timestamp}
              pathOptions={{ color: '#1d4ed8', fillColor: '#ffffff', fillOpacity: 1, weight: 2 }}
              radius={4}
            >
              <Popup>
                <strong>Route point</strong>
                <br />
                Speed: {formatSpeed(point.speedKmh)}
                <br />
                {formatDateTime(point.timestamp)}
              </Popup>
            </CircleMarker>
          ))}
          {events.map((event) => (
            <Marker key={`${event.type}-${event.timestamp}`} position={[event.latitude, event.longitude]}>
              <Popup>
                <strong>{formatEventType(event.type)}</strong>
                <br />
                Severity: {event.severity}
                <br />
                Value: {formatEventValue(event.value)}
                <br />
                {formatDateTime(event.timestamp)}
              </Popup>
            </Marker>
          ))}
          <MapBounds positions={routePositions} />
        </MapContainer>
      </div>
    </section>
  )
}

type MapBoundsProps = {
  positions: LatLngTuple[]
}

function MapBounds({ positions }: MapBoundsProps) {
  const map = useMap()

  useEffect(() => {
    if (positions.length === 1) {
      map.setView(positions[0], 16)
      return
    }

    map.fitBounds(positions, { padding: [28, 28] })
  }, [map, positions])

  return null
}
