import {
  CartesianGrid,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import type { RoutePoint } from '../types/drive'
import { formatDateTime, formatSpeed } from '../utils/formatters'

type SpeedChartProps = {
  route: RoutePoint[]
}

type SpeedChartPoint = {
  label: string
  speedKmh: number
  timestamp: string
}

export function SpeedChart({ route }: SpeedChartProps) {
  const chartData = route
    .filter((point) => point.speedKmh !== null)
    .map<SpeedChartPoint>((point) => ({
      label: formatChartTime(point.timestamp),
      speedKmh: point.speedKmh!,
      timestamp: point.timestamp,
    }))

  if (chartData.length === 0) {
    return (
      <section className="chart-empty-state">
        <div>
          <p className="eyebrow">Speed chart</p>
          <strong>No speed data</strong>
          <p>This drive does not have GPS speed values.</p>
        </div>
      </section>
    )
  }

  return (
    <section className="chart-section">
      <div className="chart-header">
        <div>
          <p className="eyebrow">Speed chart</p>
          <h2>Speed over time</h2>
        </div>
        <span>{chartData.length} samples</span>
      </div>

      <div className="speed-chart">
        <ResponsiveContainer height="100%" width="100%">
          <LineChart data={chartData} margin={{ bottom: 4, left: 8, right: 16, top: 12 }}>
            <CartesianGrid stroke="#dde2e8" strokeDasharray="4 4" />
            <XAxis
              dataKey="label"
              minTickGap={28}
              stroke="#647084"
              tick={{ fontSize: 12 }}
              tickLine={false}
            />
            <YAxis
              stroke="#647084"
              tick={{ fontSize: 12 }}
              tickFormatter={(value) => `${value}`}
              tickLine={false}
              unit=" km/h"
              width={72}
            />
            <Tooltip content={<SpeedTooltip />} />
            <Line
              activeDot={{ r: 5 }}
              dataKey="speedKmh"
              dot={false}
              name="Speed"
              stroke="#2563eb"
              strokeWidth={3}
              type="monotone"
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </section>
  )
}

type TooltipPayload = {
  payload: SpeedChartPoint
  value: number
}

type SpeedTooltipProps = {
  active?: boolean
  payload?: TooltipPayload[]
}

function SpeedTooltip({ active, payload }: SpeedTooltipProps) {
  if (!active || !payload || payload.length === 0) {
    return null
  }

  const point = payload[0].payload

  return (
    <div className="chart-tooltip">
      <strong>{formatSpeed(payload[0].value)}</strong>
      <span>{formatDateTime(point.timestamp)}</span>
    </div>
  )
}

function formatChartTime(value: string) {
  return new Intl.DateTimeFormat('en', {
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
  }).format(new Date(value))
}
