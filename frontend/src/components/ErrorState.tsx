type ErrorStateProps = {
  message: string
  onRetry: () => void
}

export function ErrorState({ message, onRetry }: ErrorStateProps) {
  return (
    <div className="state-panel state-panel-error" role="alert">
      <div>
        <strong>Unable to load drives</strong>
        <p>{message}</p>
      </div>
      <button type="button" onClick={onRetry}>
        Retry
      </button>
    </div>
  )
}
