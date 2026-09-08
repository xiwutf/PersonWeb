import { describe, it, expect } from 'vitest'
import { Arena } from '../../utils/arena/engine'
describe('Arena gameplay', () => {
  it('normalizes diagonal movement and respects boundaries', () => {
    const a = new Arena(), x = a.x, y = a.y
    a.step(.04,new Set(['KeyW','KeyD']),{x:0,y:0},false)
    expect(Math.hypot(a.x-x,a.y-y)).toBeCloseTo(a.speed*.04)
    a.x=1; a.step(.04,new Set(['KeyA']),{x:0,y:0},false); expect(a.x).toBe(20)
  })
  it('kills enemies, offers upgrades and resumes after a valid selection', () => {
    const a = new Arena(); a.xp=a.targetXp-1
    a.enemies.push({x:a.x+40,y:a.y,hp:1,r:20,speed:0,kind:0})
    a.step(.02,new Set(),{x:a.x+100,y:a.y},true)
    expect(a.kills).toBe(1); expect(a.status).toBe('upgrade'); expect(a.choices).toHaveLength(3)
    const time=a.time; a.step(.04,new Set(),{x:0,y:0},true); expect(a.time).toBe(time)
    a.choose('invalid'); expect(a.status).toBe('upgrade')
    a.choose(a.choices[0]!.id); expect(a.status).toBe('playing')
  })
  it('handles contact damage, invulnerability, death and fresh restart', () => {
    const a=new Arena(); a.enemies.push({x:a.x,y:a.y,hp:100,r:60,speed:0,kind:2})
    a.step(.01,new Set(),{x:0,y:0},false); expect(a.hp).toBe(85)
    a.step(.01,new Set(),{x:0,y:0},false); expect(a.hp).toBe(85)
    a.hp=1; a.invulnerable=0; a.step(.01,new Set(),{x:0,y:0},false); expect(a.status).toBe('over')
    expect(new Arena().hp).toBe(100)
  })
  it('freezes paused simulation and wins at three minutes', () => {
    const a=new Arena(); a.status='paused'; a.step(.04,new Set(),{x:0,y:0},true)
    expect(a.time).toBe(0); expect(a.bullets).toHaveLength(0)
    a.status='playing'; a.time=179.99; a.step(.02,new Set(),{x:0,y:0},false); expect(a.status).toBe('won')
  })
})
