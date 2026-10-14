import { describe, expect, it, vi } from 'vitest'
import { ReadingProgressSync, type ReadingProgress, type ProgressWrite } from '../../utils/library-progress'
const position = (cfi = 'epubcfi(/6/2!/4/2/1:8)', progression = .6) => ({ contentVersion: 1, cfi, chapterHref: 'chapter.xhtml', progression })
const ack = (input: ProgressWrite): ReadingProgress => ({ ...input, bookId: 1, revision: input.expectedRevision + 1, lastReadAt: '', updatedAt: '' })
describe('progress synchronization', () => {
  it('does not write on initialization, coalesces navigation and confirms only server acknowledgement', async () => {
    const save = vi.fn(async (input: ProgressWrite) => ack(input))
    const sync = new ReadingProgressSync(save, async () => null, () => {}, null, 100000)
    expect(save).not.toHaveBeenCalled()
    sync.record(position()); sync.record(position('epubcfi(/6/2!/4/2/1:20)', .7))
    expect(sync.status).toBe('pending'); await sync.flush(); sync.stop()
    expect(save).toHaveBeenCalledTimes(1); expect(sync.status).toBe('synced'); expect(sync.revision).toBe(1)
    expect(save.mock.calls[0][0].progression).toBe(.7)
  })
  it('retries a failed write with the same idempotency key and expected revision', async () => {
    const save = vi.fn().mockRejectedValueOnce(new Error('network')).mockImplementation(async (input: ProgressWrite) => ack(input))
    const sync = new ReadingProgressSync(save, async () => null, () => {}, null, 100000)
    sync.record(position()); await sync.flush(); expect(sync.status).toBe('failed')
    await sync.flush(true); sync.stop()
    expect(save.mock.calls[0][0]).toEqual(save.mock.calls[1][0]); expect(save.mock.calls[1][1]).toBe(true)
    expect(sync.status).toBe('synced')
  })
  it('retains local backreading after conflict and uses freshly fetched revision only after explicit choice', async () => {
    const remote = { ...position(), bookId: 1, revision: 4, lastReadAt: '', updatedAt: '' }
    const save = vi.fn().mockRejectedValueOnce({ response: { status: 409 } }).mockImplementation(async (input: ProgressWrite) => ack(input))
    const sync = new ReadingProgressSync(save, async () => remote, () => {}, null, 100000)
    sync.record(position('epubcfi(/6/2!/4/2/1:2)', .1)); await sync.flush()
    expect(sync.status).toBe('conflict'); await sync.flush(); expect(save).toHaveBeenCalledTimes(1)
    await sync.chooseLocal(); sync.stop()
    expect(save.mock.calls[1][0]).toMatchObject({ expectedRevision: 4, progression: .1 }); expect(sync.revision).toBe(5)
  })
  it('uses remote choice without uploading the stale local position', async () => {
    const remote = { ...position(), bookId: 1, revision: 2, lastReadAt: '', updatedAt: '' }
    const save = vi.fn().mockRejectedValue({ response: { status: 409 } })
    const sync = new ReadingProgressSync(save, async () => remote, () => {}, null, 100000)
    sync.record(position()); await sync.flush(); expect(await sync.chooseRemote()).toEqual(remote)
    await sync.flush(); sync.stop(); expect(save).toHaveBeenCalledTimes(1)
  })
  it('serializes updates that arrive during an in-flight request', async () => {
    let release!: (value: ReadingProgress) => void
    const save = vi.fn().mockImplementationOnce(() => new Promise(resolve => { release = resolve })).mockImplementation(async (input: ProgressWrite) => ack(input))
    const sync = new ReadingProgressSync(save, async () => null, () => {}, null, 100000)
    sync.record(position()); const pending = sync.flush(); sync.record(position('epubcfi(/6/2!/4/2/1:40)', .8))
    release(ack(save.mock.calls[0][0])); await pending; sync.stop()
    expect(save.mock.calls[1][0]).toMatchObject({ expectedRevision: 1, progression: .8 })
  })
  it('stops writes on a content version mismatch', async () => {
    const save = vi.fn().mockRejectedValue({ response: { status: 409 }, data: { error: 'content_version_mismatch' } })
    const sync = new ReadingProgressSync(save, async () => null, () => {}, null, 100000)
    sync.record(position()); await sync.flush(); sync.record(position()); await sync.flush(); sync.stop()
    expect(sync.status).toBe('version'); expect(save).toHaveBeenCalledTimes(1)
  })
})
