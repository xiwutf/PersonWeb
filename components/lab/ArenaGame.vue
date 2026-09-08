<template>
  <div ref="host" class="arena-console" tabindex="0" aria-label="竞技场，WASD 移动，鼠标瞄准，按住左键射击，P 暂停" @keydown="keyDown" @keyup="keyUp" @focusout="focusOut">
    <div class="arena-hud">
      <div><span class="arena-label">生命值</span><strong>{{ hud.hp }} / {{ hud.maxHp }}</strong><progress :value="hud.hp" :max="hud.maxHp" aria-label="生命值" /></div>
      <div><span class="arena-label">波次</span><strong>0{{ hud.wave }}</strong></div>
      <div><span class="arena-label">存活 / 03:00</span><strong>{{ clock }}</strong></div>
      <div><span class="arena-label">击破</span><strong>{{ hud.kills }}</strong></div>
      <AppButton size="sm" variant="secondary" :disabled="!['playing', 'paused'].includes(hud.status)" @click="togglePause">{{ hud.status === 'paused' ? '继续' : '暂停' }}</AppButton>
    </div>
    <div class="arena-screen">
      <canvas ref="canvas" :width="W" :height="H" @pointermove="pointer" @pointerdown="shoot" @pointerup="firing = false" @pointercancel="pause" @contextmenu.prevent aria-label="霓虹防线游戏画面" />
      <div v-if="hud.status !== 'playing'" class="arena-overlay">
        <div v-if="hud.status === 'upgrade'" class="arena-dialog" role="group" aria-label="选择升级">
          <span class="arena-eyebrow">SYSTEM UPGRADE / LV. {{ hud.level }}</span>
          <h3>进化，迎接下一波。</h3><p>战场已暂停，选择一项强化继续作战。</p>
          <div class="arena-choices"><button v-for="(choice, index) in choices" :key="choice.id" class="arena-choice" @click="choose(choice.id)"><span>{{ choice.icon }}</span><small>0{{ index + 1 }}</small><strong>{{ choice.name }}</strong><p>{{ choice.desc }}</p></button></div>
        </div>
        <div v-else class="arena-dialog" aria-live="polite">
          <span class="arena-eyebrow">{{ hud.status === 'paused' ? 'SIGNAL ON HOLD' : 'RUN COMPLETE' }}</span>
          <h3>{{ hud.status === 'paused' ? '喘口气，再出发。' : hud.status === 'won' ? '防线守住了！' : '信号中断' }}</h3>
          <p>{{ hud.status === 'paused' ? '点击继续回到战场。切换窗口会自动暂停。' : `存活 ${clock} · 击破 ${hud.kills} · 等级 ${hud.level}` }}</p>
          <AppButton @click="hud.status === 'paused' ? togglePause() : restart()">{{ hud.status === 'paused' ? '继续作战' : '再来一局' }}</AppButton>
        </div>
      </div>
    </div>
    <div class="arena-footer"><span>LV. {{ hud.level }}</span><progress :value="hud.xp" :max="hud.targetXp" aria-label="升级进度" /><span>{{ hud.xp }} / {{ hud.targetXp }}</span><span>WASD 移动 · 按住左键射击 · P 暂停</span></div>
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount, ref } from 'vue'
import AppButton from '~/components/ui/AppButton.vue'
import { Arena, W, H } from '~/utils/arena/engine'
const host = ref<HTMLElement>(), canvas = ref<HTMLCanvasElement>()
let game = new Arena(), raf = 0, previous = 0, firing = false
const keys = new Set<string>(), aim = { x: W / 2 + 100, y: H / 2 }
const snapshot = () => ({ hp: game.hp, maxHp: game.maxHp, wave: game.wave, time: game.time, kills: game.kills, level: game.level, xp: game.xp, targetXp: game.targetXp, status: game.status })
const hud = ref(snapshot()), choices = ref(game.choices)
const clock = computed(() => `${Math.floor(hud.value.time / 60).toString().padStart(2, '0')}:${Math.floor(hud.value.time % 60).toString().padStart(2, '0')}`)
function sync() { hud.value = snapshot(); choices.value = game.choices }
function pause() { if (game.status === 'playing') game.status = 'paused'; keys.clear(); firing = false; sync() }
function togglePause() { if (game.status === 'playing') pause(); else if (game.status === 'paused') { game.status = 'playing'; sync(); host.value?.focus({ preventScroll: true }) } }
function restart() { game = new Arena(); keys.clear(); firing = false; sync(); host.value?.focus({ preventScroll: true }) }
function choose(id: string) { game.choose(id); keys.clear(); firing = false; sync(); host.value?.focus({ preventScroll: true }) }
function keyDown(e: KeyboardEvent) { if (['KeyW','KeyA','KeyS','KeyD','KeyP','Escape'].includes(e.code)) { e.preventDefault(); if (!e.repeat && ['KeyP','Escape'].includes(e.code)) togglePause(); else keys.add(e.code) } }
function keyUp(e: KeyboardEvent) { keys.delete(e.code) }
function focusOut(e: FocusEvent) { if (!host.value?.contains(e.relatedTarget as Node)) pause() }
function pointer(e: PointerEvent) { const r = canvas.value!.getBoundingClientRect(); aim.x = (e.clientX - r.left) * W / r.width; aim.y = (e.clientY - r.top) * H / r.height }
function shoot(e: PointerEvent) { if (e.button !== 0) return; pointer(e); host.value?.focus({ preventScroll: true }); canvas.value?.setPointerCapture(e.pointerId); firing = game.status === 'playing' }
function visibility() { if (document.hidden) pause() }
// Fixed game-art palette keeps enemy silhouettes legible independently of the site theme.
const colors = { bg: '#070e1c', grid: '#14233a', cyan: '#65f5df', pink: '#ff567d', amber: '#ffbe67', violet: '#af8cff', white: '#e6fcff' }
function draw(ctx: CanvasRenderingContext2D) {
  ctx.fillStyle = colors.bg; ctx.fillRect(0, 0, W, H)
  ctx.strokeStyle = colors.grid; ctx.lineWidth = 1
  for (let x = 0; x < W; x += 44) { ctx.beginPath(); ctx.moveTo(x,0); ctx.lineTo(x,H); ctx.stroke() }
  for (let y = 0; y < H; y += 44) { ctx.beginPath(); ctx.moveTo(0,y); ctx.lineTo(W,y); ctx.stroke() }
  ctx.strokeStyle = colors.cyan; ctx.globalAlpha = .2; ctx.strokeRect(12,12,W-24,H-24)
  ctx.beginPath(); ctx.arc(W/2,H/2,150,0,Math.PI*2); ctx.stroke(); ctx.globalAlpha = 1
  for (const e of game.enemies) {
    ctx.save(); ctx.translate(e.x,e.y); ctx.rotate(Math.atan2(game.y-e.y,game.x-e.x)); ctx.strokeStyle = [colors.pink,colors.amber,colors.violet][e.kind]!; ctx.fillStyle = colors.bg; ctx.lineWidth = 2; ctx.shadowColor = ctx.strokeStyle; ctx.shadowBlur = 12
    ctx.beginPath(); const sides = e.kind === 2 ? 6 : e.kind === 1 ? 3 : 4
    for(let i=0;i<sides;i++) { const a=i*Math.PI*2/sides; i ? ctx.lineTo(Math.cos(a)*e.r,Math.sin(a)*e.r) : ctx.moveTo(Math.cos(a)*e.r,Math.sin(a)*e.r) }
    ctx.closePath(); ctx.fill(); ctx.stroke(); ctx.fillStyle=ctx.strokeStyle; ctx.fillRect(-3,-3,6,6); ctx.restore()
  }
  ctx.fillStyle = colors.cyan; ctx.shadowColor=colors.cyan; ctx.shadowBlur=12
  for (const b of game.bullets) { ctx.beginPath(); ctx.arc(b.x,b.y,3,0,Math.PI*2); ctx.fill() }
  for (const s of game.sparks) { ctx.globalAlpha=s.life/.4; ctx.fillRect(s.x,s.y,3,3) } ctx.globalAlpha=1
  ctx.save(); ctx.translate(game.x,game.y); ctx.rotate(Math.atan2(aim.y-game.y,aim.x-game.x)); ctx.fillStyle=colors.cyan
  if(game.invulnerable>0) ctx.globalAlpha=.45 + Math.sin(game.time*35)*.25
  ctx.beginPath(); ctx.moveTo(20,0); ctx.lineTo(-12,-13); ctx.lineTo(-6,0); ctx.lineTo(-12,13); ctx.closePath(); ctx.fill(); ctx.fillStyle=colors.white; ctx.fillRect(-3,-3,6,6); ctx.restore(); ctx.shadowBlur=0
  ctx.strokeStyle=colors.white; ctx.lineWidth=1; ctx.beginPath(); ctx.arc(aim.x,aim.y,8,0,Math.PI*2); ctx.moveTo(aim.x-13,aim.y); ctx.lineTo(aim.x+13,aim.y); ctx.moveTo(aim.x,aim.y-13); ctx.lineTo(aim.x,aim.y+13); ctx.stroke()
}
onMounted(() => {
  const ctx = canvas.value?.getContext('2d'); if (!ctx) return
  host.value?.focus({ preventScroll: true })
  function frame(now: number) { game.step(previous ? (now-previous)/1000 : 0,keys,aim,firing); previous=now; sync(); draw(ctx!); raf=requestAnimationFrame(frame) }
  raf=requestAnimationFrame(frame); window.addEventListener('blur',pause); document.addEventListener('visibilitychange',visibility)
})
onBeforeUnmount(() => { cancelAnimationFrame(raf); window.removeEventListener('blur',pause); document.removeEventListener('visibilitychange',visibility) })
</script>
