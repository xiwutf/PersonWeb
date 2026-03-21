let motionOneDomPromise: Promise<typeof import('@motionone/dom')> | null = null

export const loadMotionOneDom = async () => {
  if (!motionOneDomPromise) {
    motionOneDomPromise = import('@motionone/dom')
  }

  return motionOneDomPromise
}
