export function LoadingState() {
  return (
    <div className="state-panel state-panel-loading" role="status">
      <div>
        <strong>Loading data</strong>
        <p>Please wait a moment.</p>
      </div>
      <div className="loading-indicator" aria-hidden="true">
        <span />
        <span />
        <span />
      </div>
    </div>
  )
}
