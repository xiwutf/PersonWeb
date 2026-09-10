<template>
  <section v-if="open" class="life-margin-scanner" aria-labelledby="margin-scanner-title">
    <header class="life-margin-scanner-head">
      <div>
        <p class="life-margin-focus-kicker">FROM PAPER</p>
        <h2 id="margin-scanner-title">从纸上摘下来</h2>
      </div>
      <button type="button" class="life-admin-add" @click="close">收起</button>
    </header>

    <div v-if="!previewUrl" class="life-margin-scan-pick">
      <p>拍一张写满句子的纸，尽量摆正、光线均匀。</p>
      <label class="life-margin-scan-button">
        <span>拍照或选图</span>
        <input
          type="file"
          accept="image/jpeg,image/png,image/webp"
          capture="environment"
          @change="onFileChange"
        >
      </label>
    </div>

    <template v-else>
      <figure class="life-margin-paper-preview">
        <img :src="previewUrl" alt="待识别的手写纸张">
        <figcaption>
          <span>原纸</span>
          <label class="life-margin-scan-replace">
            换一张
            <input
              type="file"
              accept="image/jpeg,image/png,image/webp"
              capture="environment"
              @change="onFileChange"
            >
          </label>
        </figcaption>
      </figure>

      <div class="life-margin-scan-actions">
        <button
          type="button"
          class="life-admin-add life-admin-add--primary"
          :disabled="recognizing"
          @click="recognize"
        >
          {{ recognizing ? '正在读纸…' : '读出纸上的句子' }}
        </button>
        <p v-if="errorMessage" role="alert" class="life-margin-scan-error">{{ errorMessage }}</p>
      </div>

      <section v-if="sentences.length" class="life-margin-scan-result" aria-labelledby="margin-scan-result-title">
        <header>
          <h3 id="margin-scan-result-title">识别出的句子</h3>
          <span>{{ sentences.length }} 句</span>
        </header>
        <ol>
          <li v-for="(sentence, index) in sentences" :key="index">
            <span aria-hidden="true">{{ String(index + 1).padStart(2, '0') }}</span>
            <textarea v-model="sentences[index]" rows="2" maxlength="400" :aria-label="`第 ${index + 1} 句`" />
            <button type="button" aria-label="删除这句" @click="sentences.splice(index, 1)">×</button>
          </li>
        </ol>
        <div class="life-margin-scan-confirm">
          <p>先改到和纸上一样，再收入「随手记」。</p>
          <div class="life-margin-scan-submit">
            <button
              type="button"
              class="life-admin-add life-admin-add--primary"
              :disabled="uploading || !validSentences.length"
              @click="confirm"
            >
              {{ uploading ? '正在保存原纸…' : `保存原纸和这 ${validSentences.length} 句` }}
            </button>
            <span v-if="confirmMessage" role="status" class="life-margin-scan-error">
              {{ confirmMessage }}
            </span>
          </div>
        </div>
      </section>
    </template>
  </section>
</template>

<script setup lang="ts">
type ExtractResult = {
  sentences: string[]
}

type MediaUploadResult = {
  url: string
}

type ScanImport = {
  imageUrl: string
  sentences: string[]
}

const props = defineProps<{
  open: boolean
}>()

const emit = defineEmits<{
  close: []
  confirm: [result: ScanImport]
}>()

const api = useApi()
const selectedFile = ref<File | null>(null)
const previewUrl = ref('')
const recognizing = ref(false)
const uploading = ref(false)
const errorMessage = ref('')
const confirmMessage = ref('')
const sentences = ref<string[]>([])
const validSentences = computed(() => sentences.value.map(item => item.trim()).filter(Boolean))

function releasePreview() {
  if (previewUrl.value) URL.revokeObjectURL(previewUrl.value)
  previewUrl.value = ''
}

function onFileChange(event: Event) {
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  input.value = ''
  if (!file) return

  errorMessage.value = ''
  confirmMessage.value = ''
  if (!['image/jpeg', 'image/png', 'image/webp'].includes(file.type)) {
    errorMessage.value = '请使用 JPG、PNG 或 WebP 图片。'
    return
  }
  if (file.size > 10 * 1024 * 1024) {
    errorMessage.value = '图片不要超过 10MB。'
    return
  }

  releasePreview()
  selectedFile.value = file
  previewUrl.value = URL.createObjectURL(file)
  sentences.value = []
}

async function recognize() {
  if (!selectedFile.value || recognizing.value) return
  recognizing.value = true
  errorMessage.value = ''

  try {
    const form = new FormData()
    form.append('file', selectedFile.value)
    const result = await api.post<ExtractResult>('Handwriting/extract', form, { silent: true })
    sentences.value = result.sentences
    if (!sentences.value.length) errorMessage.value = '没有读到完整句子，可以换一张更清楚的照片。'
  } catch (error) {
    errorMessage.value = error instanceof Error ? error.message : '这张纸暂时没有读出来，请再试一次。'
  } finally {
    recognizing.value = false
  }
}

async function confirm() {
  if (!selectedFile.value || !validSentences.value.length || uploading.value) return
  uploading.value = true
  errorMessage.value = ''
  confirmMessage.value = '正在上传原纸…'
  try {
    const form = new FormData()
    form.append('file', selectedFile.value)
    const uploaded = await api.post<MediaUploadResult>('Media/upload', form, { silent: true })
    confirmMessage.value = '原纸已上传，正在保存句子…'
    emit('confirm', { imageUrl: uploaded.url, sentences: validSentences.value })
  } catch (error) {
    confirmMessage.value = error instanceof Error ? error.message : '原纸没有保存成功，请再试一次。'
  } finally {
    uploading.value = false
  }
}

function close() {
  emit('close')
  reset()
}

function reset() {
  releasePreview()
  selectedFile.value = null
  sentences.value = []
  errorMessage.value = ''
  confirmMessage.value = ''
  recognizing.value = false
  uploading.value = false
}

watch(() => props.open, (open) => {
  if (!open) reset()
})

onBeforeUnmount(releasePreview)
</script>
