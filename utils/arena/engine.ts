export type Enemy = { x: number; y: number; hp: number; r: number; speed: number; kind: number }
export const W = 1100, H = 620
export const upgrades = [
  { id: 'power', icon: 'ϟ', name: '高能弹芯', desc: '伤害 +35%，击碎重装敌人。' },
  { id: 'rate', icon: '»', name: '极速扳机', desc: '射击间隔缩短 18%。' },
  { id: 'spread', icon: '⋔', name: '分裂弹道', desc: '额外发射 2 枚散射子弹。' },
  { id: 'speed', icon: '↗', name: '轻量推进', desc: '移动速度 +15%。' },
  { id: 'heal', icon: '+', name: '纳米修复', desc: '恢复 45 生命，上限 +15。' }
]
export class Arena {
  x = W / 2; y = H / 2; hp = 100; maxHp = 100; time = 0; kills = 0; level = 1; xp = 0
  damage = 22; interval = .19; speed = 235; shots = 1; invulnerable = 0; cooldown = 0; spawnClock = .4
  status: 'playing' | 'paused' | 'upgrade' | 'over' | 'won' = 'playing'
  choices: typeof upgrades = []
  enemies: Enemy[] = []
  bullets: { x: number; y: number; vx: number; vy: number; life: number }[] = []
  sparks: { x: number; y: number; vx: number; vy: number; life: number }[] = []
  get wave() { return Math.floor(this.time / 20) + 1 }
  get targetXp() { return 4 + this.level * 2 }
  choose(id: string) {
    if (this.status !== 'upgrade' || !this.choices.some(c => c.id === id)) return
    if (id === 'power') this.damage *= 1.35
    if (id === 'rate') this.interval = Math.max(.055, this.interval * .82)
    if (id === 'spread') this.shots = Math.min(9, this.shots + 2)
    if (id === 'speed') this.speed = Math.min(450, this.speed * 1.15)
    if (id === 'heal') { this.maxHp += 15; this.hp = Math.min(this.maxHp, this.hp + 45) }
    this.status = 'playing'; this.invulnerable = 1
  }
  step(dt: number, keys: Set<string>, aim: { x: number; y: number }, firing: boolean) {
    if (this.status !== 'playing') return
    dt = Math.min(dt, .04); this.time += dt
    if (this.time >= 180) { this.status = 'won'; return }
    this.invulnerable -= dt; this.cooldown -= dt; this.spawnClock -= dt
    let dx = Number(keys.has('KeyD')) - Number(keys.has('KeyA'))
    let dy = Number(keys.has('KeyS')) - Number(keys.has('KeyW'))
    const len = Math.hypot(dx, dy) || 1
    this.x = Math.max(20, Math.min(W - 20, this.x + dx / len * this.speed * dt))
    this.y = Math.max(20, Math.min(H - 20, this.y + dy / len * this.speed * dt))
    if (firing && this.cooldown <= 0) {
      this.cooldown = this.interval
      for (let i = 0; i < this.shots; i++) {
        const a = Math.atan2(aim.y - this.y, aim.x - this.x) + (i - (this.shots - 1) / 2) * .13
        this.bullets.push({ x: this.x + Math.cos(a) * 22, y: this.y + Math.sin(a) * 22, vx: Math.cos(a) * 720, vy: Math.sin(a) * 720, life: 1.4 })
      }
    }
    if (this.spawnClock <= 0 && this.enemies.length < 85) {
      this.spawnClock = Math.max(.22, .95 - this.wave * .08)
      const side = Math.floor(Math.random() * 4), kind = this.wave > 1 ? Math.floor(Math.random() * 3) : 0
      this.enemies.push({ x: side < 2 ? (side === 0 ? -20 : W + 20) : Math.random() * W, y: side >= 2 ? (side === 2 ? -20 : H + 20) : Math.random() * H, kind, r: [13, 9, 21][kind]!, hp: [32, 20, 95][kind]! + this.wave * 3, speed: [65, 125, 40][kind]! + this.wave * 5 })
    }
    for (const b of this.bullets) {
      b.x += b.vx * dt; b.y += b.vy * dt; b.life -= dt
      for (const e of this.enemies) {
        if (b.life <= 0 || e.hp <= 0 || Math.hypot(b.x - e.x, b.y - e.y) > e.r + 5) continue
        b.life = 0; e.hp -= this.damage
        if (e.hp <= 0) {
          this.kills++; this.xp++
          for (let i = 0; i < 8; i++) { const a = Math.random() * Math.PI * 2; this.sparks.push({ x: e.x, y: e.y, vx: Math.cos(a) * 110, vy: Math.sin(a) * 110, life: .4 }) }
        }
      }
    }
    for (const e of this.enemies) {
      if (e.hp <= 0) continue
      const a = Math.atan2(this.y - e.y, this.x - e.x)
      e.x += Math.cos(a) * e.speed * dt; e.y += Math.sin(a) * e.speed * dt
      if (Math.hypot(e.x - this.x, e.y - this.y) < e.r + 13 && this.invulnerable <= 0) {
        this.hp = Math.max(0, this.hp - 15); this.invulnerable = .85
        e.x -= Math.cos(a) * 35; e.y -= Math.sin(a) * 35
      }
    }
    for (const s of this.sparks) { s.x += s.vx * dt; s.y += s.vy * dt; s.life -= dt }
    this.bullets = this.bullets.filter(b => b.life > 0)
    this.enemies = this.enemies.filter(e => e.hp > 0)
    this.sparks = this.sparks.filter(s => s.life > 0).slice(-200)
    if (this.hp <= 0) this.status = 'over'
    else if (this.xp >= this.targetXp) {
      this.xp -= this.targetXp; this.level++; this.status = 'upgrade'
      this.choices = [...upgrades].filter(c => c.id !== 'spread' || this.shots < 9).sort(() => Math.random() - .5).slice(0, 3)
    }
  }
}
