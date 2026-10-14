export type ReadingPosition = { contentVersion: number; cfi: string; chapterHref: string; progression: number }
export type ReadingProgress = ReadingPosition & { bookId: number; revision: number; lastReadAt: string; updatedAt: string }
export type ProgressWrite = ReadingPosition & { expectedRevision: number; sessionId: string }
export type SyncStatus = 'idle' | 'pending' | 'saving' | 'synced' | 'failed' | 'conflict' | 'version'

// One writer per open reader. Positions only enter here after an intentional navigation.
export class ReadingProgressSync {
  revision = 0
  status: SyncStatus = 'idle'
  remote: ReadingProgress | null = null
  private pending?: ReadingPosition
  private attempt?: ProgressWrite
  private timer?: ReturnType<typeof setTimeout>
  private flight?: Promise<void>
  private stopped = false
  constructor(private save: (input: ProgressWrite, keepalive: boolean) => Promise<ReadingProgress>,
    private get: () => Promise<ReadingProgress | null>, private changed: () => void,
    initial: ReadingProgress | null, private delay = 1000) { this.revision = initial?.revision ?? 0 }
  record(position: ReadingPosition) {
    if (this.stopped || this.status === 'version') return
    this.pending = { ...position }
    if (this.status !== 'conflict') {
      this.status = 'pending'; clearTimeout(this.timer)
      this.timer = setTimeout(() => { void this.flush() }, this.delay)
    }
    this.changed()
  }
  flush(keepalive = false): Promise<void> {
    clearTimeout(this.timer)
    if (this.status === 'conflict' || this.status === 'version') return Promise.resolve()
    if (this.flight) return this.flight.then(() => this.flush(keepalive))
    if (!this.attempt && !this.pending) return Promise.resolve()
    if (!this.attempt) {
      this.attempt = { ...this.pending!, expectedRevision: this.revision, sessionId: crypto.randomUUID() }
      this.pending = undefined
    }
    const input = this.attempt
    this.status = 'saving'; this.changed()
    this.flight = (async () => {
      try {
        const acknowledged = await this.save(input, keepalive)
        this.revision = acknowledged.revision; this.attempt = undefined
        this.status = this.pending ? 'pending' : 'synced'
      } catch (cause) {
        const failure = cause as { response?: { status?: number }; statusCode?: number; data?: { error?: string } }
        if (failure.data?.error === 'content_version_mismatch') this.status = 'version'
        else if ((failure.response?.status ?? failure.statusCode) === 409) {
          this.status = 'conflict'
          try { this.remote = await this.get() } catch { /* Retry the read on explicit resolution. */ }
        } else this.status = 'failed'
      } finally { this.flight = undefined; this.changed() }
    })()
    return this.flight.then(() => {
      if (this.pending && this.status === 'pending') return this.flush(keepalive)
    })
  }
  async chooseLocal() {
    const remote = await this.get()
    const local = this.pending ?? this.attempt
    if (!local) return
    if (remote && remote.contentVersion !== local.contentVersion) { this.status = 'version'; this.changed(); return }
    this.revision = remote?.revision ?? 0; this.remote = remote
    this.pending = { contentVersion: local.contentVersion, cfi: local.cfi, chapterHref: local.chapterHref, progression: local.progression }
    this.attempt = undefined; this.status = 'pending'
    await this.flush()
  }
  async chooseRemote(): Promise<ReadingProgress | null> {
    const remote = await this.get()
    this.remote = remote; this.revision = remote?.revision ?? 0
    this.pending = undefined; this.attempt = undefined; this.status = 'idle'; this.changed()
    return remote
  }
  stop() { this.stopped = true; clearTimeout(this.timer) }
}
